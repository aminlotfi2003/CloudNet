(() => {
    const uploadForms = document.querySelectorAll('[data-upload-form]');
    uploadForms.forEach((form) => {
        form.addEventListener('submit', () => {
            const button = form.querySelector('[data-upload-button]');
            const status = form.querySelector('[data-upload-status]');
            if (button) {
                button.disabled = true;
                button.textContent = 'Uploading...';
            }
            if (status) {
                status.textContent = 'Uploading file. Please wait.';
            }
        });
    });

    document.querySelectorAll('[data-copy-target]').forEach((button) => {
        button.addEventListener('click', async () => {
            const targetId = button.getAttribute('data-copy-target');
            const target = targetId ? document.getElementById(targetId) : null;
            if (!target) {
                return;
            }

            try {
                await navigator.clipboard.writeText(target.textContent ?? '');
                button.textContent = 'Copied!';
                setTimeout(() => (button.textContent = 'Copy token'), 1500);
            } catch {
                button.textContent = 'Copy failed';
            }
        });
    });
})();