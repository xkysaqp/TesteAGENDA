// AgendaApp - Scripts customizados

$(document).ready(function() {
    // Inicialização de tooltips do Bootstrap
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Inicialização de popovers do Bootstrap
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Configurar modal de confirmação de exclusão via Ajax
    setupDeleteModal();

    // Fade out para mensagens de alerta após 5 segundos
    $('.alert').delay(5000).fadeOut('slow');
});

// Função para formatação de CNPJ em campos de entrada
function formatarCNPJ(input) {
    let value = input.value.replace(/\D/g, '');
    value = value.replace(/^(\d{2})(\d)/, '$1.$2');
    value = value.replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3');
    value = value.replace(/\.(\d{3})(\d)/, '.$1/$2');
    value = value.replace(/(\d{4})(\d)/, '$1-$2');
    input.value = value;
}

// Função para validação de email
function validarEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

// Configuração do modal de confirmação de exclusão
function setupDeleteModal() {
    // Interceptar cliques em botões de delete
    $(document).on('click', '.btn-delete-ajax', function(e) {
        e.preventDefault();
        
        const deleteUrl = $(this).data('delete-url');
        const itemName = $(this).data('item-name') || 'este item';
        const itemDetails = $(this).data('item-details') || '';
        const confirmMessage = $(this).data('confirm-message') || `Tem certeza que deseja excluir ${itemName}?`;
        
        showDeleteModal(deleteUrl, confirmMessage, itemDetails, itemName);
    });
}

// Exibir modal de confirmação de exclusão
function showDeleteModal(deleteUrl, message, details, itemName) {
    const modal = new bootstrap.Modal(document.getElementById('confirmDeleteModal'));
    
    // Configurar mensagem
    $('#confirmDeleteMessage').text(message);
    
    // Configurar detalhes se fornecidos
    if (details) {
        $('#confirmDeleteDetails').html(details).show();
    } else {
        $('#confirmDeleteDetails').hide();
    }
    
    // Configurar botão de confirmação
    $('#confirmDeleteBtn').off('click').on('click', function() {
        executeDelete(deleteUrl, itemName, modal);
    });
    
    // Exibir modal
    modal.show();
}

// Executar exclusão via Ajax
function executeDelete(deleteUrl, itemName, modal) {
    const btn = $('#confirmDeleteBtn');
    const originalText = btn.html();
    
    // Desabilitar botão e mostrar loading
    btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Excluindo...');
    
    // Obter token CSRF
    const token = $('input[name="__RequestVerificationToken"]').val();
    
    $.ajax({
        url: deleteUrl,
        type: 'POST',
        headers: {
            'RequestVerificationToken': token
        },
        data: {
            __RequestVerificationToken: token
        },
        success: function(response) {
            if (response.success) {
                // Fechar modal
                modal.hide();
                
                // Mostrar mensagem de sucesso
                showSuccessMessage(response.message);
                
                // Redirecionar ou recarregar página após delay
                setTimeout(function() {
                    // Se estamos na página de edição ou detalhes de prestador, recarregar em vez de redirecionar
                    if (window.location.pathname.includes('/Prestador/Edit') || 
                        window.location.pathname.includes('/Prestador/Details')) {
                        location.reload();
                    } else if (response.redirectUrl) {
                        window.location.href = response.redirectUrl;
                    } else {
                        location.reload();
                    }
                }, 1500);
            } else {
                // Mostrar erro
                showErrorMessage(response.message);
                
                // Reabilitar botão
                btn.prop('disabled', false).html(originalText);
            }
        },
        error: function(xhr, status, error) {
            console.error('Erro na exclusão:', error);
            
            let errorMessage = 'Erro interno do servidor. Tente novamente.';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            }
            
            showErrorMessage(errorMessage);
            
            // Reabilitar botão
            btn.prop('disabled', false).html(originalText);
        }
    });
}

// Mostrar mensagem de sucesso usando SweetAlert2
function showSuccessMessage(message) {
    Swal.fire({
        icon: 'success',
        title: 'Sucesso!',
        text: message,
        timer: 3000,
        showConfirmButton: false,
        toast: true,
        position: 'top-end'
    });
}

// Mostrar mensagem de erro usando SweetAlert2
function showErrorMessage(message) {
    Swal.fire({
        icon: 'error',
        title: 'Erro!',
        text: message,
        confirmButtonText: 'OK',
        confirmButtonColor: '#dc3545'
    });
}