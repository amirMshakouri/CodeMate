        (() => {
        const form = document.getElementById("forgot-form");
        const banner = document.getElementById("banner");
        const submitBtn = document.getElementById("submit-btn");

        form.addEventListener("submit", async (e) => {
            e.preventDefault();
            CodeMateForm.hideBanner(banner);
            CodeMateForm.clearFieldErrors(form);

            const email = document.getElementById("email").value.trim();

            CodeMateForm.setLoading(submitBtn, true, "در حال ارسال…");
            try {
                const result = await CodeMateApi.post("/auth/forgot-password", { email });
                CodeMateForm.showBanner(
                    banner,
                    result.message || "اگه این ایمیل حساب داشته باشه، کد بازیابی ارسال شد.",
                    "ok"
                );
            } catch (err) {
                if (!CodeMateForm.applyFieldErrors(form, err.errors)) {
                    CodeMateForm.showBanner(banner, err.message || "مشکلی پیش اومد.");
                }
            } finally {
                CodeMateForm.setLoading(submitBtn, false);
            }
        });
    })();
