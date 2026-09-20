        (() => {
        if (CodeMateApi.isAuthenticated()) {
            window.location.href = "dashboard.html";
            return;
        }

        const form = document.getElementById("login-form");
        const banner = document.getElementById("banner");
        const submitBtn = document.getElementById("submit-btn");

        form.addEventListener("submit", async (e) => {
            e.preventDefault();
            CodeMateForm.hideBanner(banner);
            CodeMateForm.clearFieldErrors(form);

            const usernameOrEmail = document.getElementById("userNameOrEmail").value.trim();
            const password = document.getElementById("password").value;

            if (!usernameOrEmail || !password) {
                CodeMateForm.showBanner(banner, "نام کاربری/ایمیل و رمز عبور رو وارد کن.");
                return;
            }

            CodeMateForm.setLoading(submitBtn, true, "در حال ورود…");
            try {
                const result = await CodeMateApi.post("/auth/login", { usernameOrEmail, password });
                CodeMateApi.setSession({
                    token: result.token,
                    expiration: result.expiration,
                    userName: result.userName,
                });
                window.location.href = "dashboard.html";
            } catch (err) {
                if (!CodeMateForm.applyFieldErrors(form, err.errors)) {
                    CodeMateForm.showBanner(banner, err.message || "ورود ناموفق بود.");
                }
            } finally {
                CodeMateForm.setLoading(submitBtn, false);
            }
        });
    })();
