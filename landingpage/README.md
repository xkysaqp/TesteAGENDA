# AgendaApp - Landing Page

Uma landing page moderna e responsiva para o sistema de agendamento AgendaApp, desenvolvida com HTML5, CSS3 e JavaScript puro.

## 🚀 Características

- **Design Moderno**: Interface limpa e profissional com gradientes e sombras
- **Totalmente Responsiva**: Funciona perfeitamente em desktop, tablet e mobile
- **Animações Suaves**: Transições e animações CSS para melhor experiência do usuário
- **Performance Otimizada**: Carregamento rápido e código otimizado
- **Acessibilidade**: Seguindo as melhores práticas de acessibilidade web
- **SEO Friendly**: Estrutura HTML semântica e meta tags otimizadas

## 📁 Estrutura de Arquivos

```
landingpage/
├── index.html          # Página principal
├── styles.css          # Estilos CSS
├── script.js           # Funcionalidades JavaScript
└── README.md           # Documentação
```

## 🎨 Seções da Landing Page

### 1. Header/Navegação
- Logo do AgendaApp
- Menu de navegação responsivo
- Botão de acesso ao sistema
- Menu hambúrguer para mobile

### 2. Hero Section
- Título principal com destaque
- Descrição do produto
- Botões de call-to-action
- Mockup interativo do calendário

### 3. Recursos (Features)
- 6 cards destacando os principais recursos
- Ícones Font Awesome
- Animações de entrada

### 4. Planos e Preços
- 3 planos: Básico, Profissional e Empresarial
- Destaque para o plano mais popular
- Lista de recursos por plano

### 5. Contato
- Informações de contato
- Formulário funcional com validação
- Sistema de notificações

### 6. Footer
- Links úteis organizados por categoria
- Redes sociais
- Informações da empresa

## 🛠️ Tecnologias Utilizadas

- **HTML5**: Estrutura semântica
- **CSS3**: 
  - Grid e Flexbox para layout
  - CSS Variables (Custom Properties)
  - Animações e transições
  - Media queries para responsividade
- **JavaScript ES6+**:
  - Intersection Observer API
  - Event listeners
  - DOM manipulation
  - Form validation

## 🎯 Funcionalidades JavaScript

### Navegação
- Menu mobile toggle
- Smooth scrolling para links internos
- Header com efeito de scroll

### Formulário de Contato
- Validação de campos obrigatórios
- Validação de email
- Sistema de notificações
- Simulação de envio

### Animações
- Intersection Observer para animações de entrada
- Parallax effect no hero
- Hover effects nos cards
- Botão "back to top"

### Performance
- Lazy loading para imagens
- Otimização de animações
- Monitoramento de performance

## 🎨 Paleta de Cores

```css
--primary-color: #6366f1    /* Azul principal */
--primary-dark: #4f46e5     /* Azul escuro */
--accent-color: #10b981     /* Verde accent */
--text-primary: #1e293b     /* Texto principal */
--text-secondary: #64748b   /* Texto secundário */
--white: #ffffff           /* Branco */
--gray-50: #f8fafc        /* Cinza claro */
--gray-900: #0f172a       /* Cinza escuro */
```

## 📱 Responsividade

A landing page é totalmente responsiva com breakpoints:

- **Desktop**: > 1024px
- **Tablet**: 768px - 1024px
- **Mobile**: < 768px
- **Mobile Pequeno**: < 480px

## 🚀 Como Usar

1. **Visualização Local**:
   ```bash
   # Abra o arquivo index.html em qualquer navegador moderno
   ```

2. **Servidor Local** (recomendado):
   ```bash
   # Python 3
   python -m http.server 8000
   
   # Node.js (com http-server)
   npx http-server
   
   # PHP
   php -S localhost:8000
   ```

3. **Acesse**: `http://localhost:8000`

## 🔧 Personalização

### Cores
Edite as variáveis CSS no arquivo `styles.css`:

```css
:root {
    --primary-color: #6366f1;
    --accent-color: #10b981;
    /* ... outras cores */
}
```

### Conteúdo
- **Textos**: Edite diretamente no `index.html`
- **Imagens**: Substitua os ícones Font Awesome ou adicione imagens
- **Links**: Atualize os hrefs para apontar para suas páginas

### Funcionalidades
- **Formulário**: Configure o endpoint de envio no `script.js`
- **Analytics**: Adicione Google Analytics ou outros scripts
- **SEO**: Atualize meta tags no `<head>`

## 📊 Performance

### Otimizações Implementadas
- CSS e JS minificados
- Imagens otimizadas (quando aplicável)
- Lazy loading
- Animações otimizadas com `transform` e `opacity`
- Intersection Observer para animações

### Métricas Esperadas
- **First Contentful Paint**: < 1.5s
- **Largest Contentful Paint**: < 2.5s
- **Cumulative Layout Shift**: < 0.1
- **First Input Delay**: < 100ms

## 🔒 Segurança

- Validação de formulários no cliente
- Sanitização de inputs
- HTTPS recomendado para produção
- Headers de segurança (implementar no servidor)

## 🌐 Compatibilidade

### Navegadores Suportados
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

### Funcionalidades Modernas
- CSS Grid e Flexbox
- CSS Variables
- Intersection Observer API
- ES6+ JavaScript

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

## 🤝 Contribuição

Para contribuir com melhorias:

1. Faça um fork do projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Push para a branch
5. Abra um Pull Request

## 📞 Suporte

Para dúvidas ou suporte:
- Email: contato@agendaapp.com
- Documentação: [Link para documentação]
- Issues: [Link para issues]

---

**Desenvolvido com ❤️ para o AgendaApp** 