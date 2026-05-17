// Mera School – site.js
// Auto-dismiss Bootstrap toasts after 4 seconds
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.toast').forEach(el => {
        const t = new bootstrap.Toast(el, { delay: 4000 });
        t.show();
    });
});
