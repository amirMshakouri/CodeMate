        (() => {
        const form = document.getElementById("reset-form");
        const banner = document.getElementById("banner");
        const submitBtn = document.getElementById("submit-btn");

        const params = new URLSearchParams(window.location.search);
        if (params.get("email")) document.getElementById("email").value = params.get("email");
        if (params.get("token")) document.getElementById("token").value = params.get("token");

        form.addEventListener("submit", async (e) => {
            e.preventDefault();
            CodeMateForm.hideBanner(banner);
            CodeMateForm.clearFieldErrors(form);

            const email = document.getElementById("email").value.trim();
            const token = document.getElementById("token").value.trim();
            const newPassword = document.getElementById("newPassword").value;
            const confirmNewPassword = document.getElementById("confirmNewPassword").value;

            if (newPassword !== confirmNewPassword) {
                CodeMateForm.showBanner(banner, "رمزهای عبور یکسان نیستن.");
                return;
            }

            CodeMateForm.setLoading(submitBtn, true, "در حال تنظیم…");
            try {
                const result = await CodeMateApi.post("/auth/reset-password", {
                    email,
                    token,
                    newPassword,
                    confirmNewPassword,
                });
                CodeMateForm.showBanner(banner, result.message || "رمز عبور تغییر کرد. در حال انتقال…", "ok");
                setTimeout(() => (window.location.href = "login.html"), 1200);
            } catch (err) {
                if (!CodeMateForm.applyFieldErrors(form, err.errors)) {
                    CodeMateForm.showBanner(banner, err.message || "بازنشانی ناموفق بود.");
                }
            } finally {
                CodeMateForm.setLoading(submitBtn, false);
            }
        });
    })();
