        const CodeMateForm = (() => {
            function showBanner(el, message, kind = "error") {
                el.textContent = message;
                el.className = `banner show ${kind === "error" ? "banner-error" : "banner-ok"}`;
            }

            function hideBanner(el) {
                el.className = "banner";
                el.textContent = "";
            }

            function clearFieldErrors(form) {
                form.querySelectorAll(".field-error").forEach((n) => {
                    n.style.display = "none";
                    n.textContent = "";
                });
            }

            function applyFieldErrors(form, errors) {
                if (!errors) return false;
                let applied = false;
                Object.entries(errors).forEach(([field, messages]) => {
                    const target = form.querySelector(`.field-error[data-for="${field.charAt(0).toLowerCase()}${field.slice(1)}"]`);
                    if (target && messages && messages.length) {
                        target.textContent = messages[0];
                        target.style.display = "block";
                        applied = true;
                    }
                });
                return applied;
            }

            function setLoading(button, loading, loadingLabel = "لطفاً صبر کنید…") {
                if (loading) {
                    button.dataset.originalLabel = button.dataset.originalLabel || button.textContent;
                    button.textContent = loadingLabel;
                    button.disabled = true;
                } else {
                    button.textContent = button.dataset.originalLabel || button.textContent;
                    button.disabled = false;
                }
            }

            return { showBanner, hideBanner, clearFieldErrors, applyFieldErrors, setLoading };
        })();
