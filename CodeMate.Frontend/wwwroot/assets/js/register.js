        (() => {
        if (CodeMateApi.isAuthenticated()) {
            window.location.href = "dashboard.html";
            return;
        }

        const form = document.getElementById("register-form");
        const banner = document.getElementById("banner");
        const submitBtn = document.getElementById("submit-btn");

        form.addEventListener("submit", async (e) => {
            e.preventDefault();
            CodeMateForm.hideBanner(banner);
            CodeMateForm.clearFieldErrors(form);

            const userName = document.getElementById("userName").value.trim();
            const email = document.getElementById("email").value.trim();
            const password = document.getElementById("password").value;
            const confirmPassword = document.getElementById("confirmPassword").value;
            const fullName = document.getElementById("fullName").value.trim();
            const phoneNumber = document.getElementById("phoneNumber").value.trim();
            const bio = document.getElementById("bio").value.trim();

            if (password !== confirmPassword) {
                CodeMateForm.showBanner(banner, "رمزهای عبور یکسان نیستن.");
                return;
            }

            CodeMateForm.setLoading(submitBtn, true, "در حال ساخت حساب…");
            try {
                await CodeMateApi.post("/auth/register", {
                    userName,
                    email,
                    password,
                    confirmPassword,
                    fullName: fullName || null,
                    phoneNumber: phoneNumber || null,
                    bio: bio || null,
                });
                CodeMateForm.showBanner(banner, "حساب ساخته شد. در حال انتقال به صفحه ورود…", "ok");
                setTimeout(() => (window.location.href = "login.html"), 1200);
            } catch (err) {
                if (!CodeMateForm.applyFieldErrors(form, err.errors)) {
                    CodeMateForm.showBanner(banner, err.message || "ثبت‌نام ناموفق بود.");
                }
            } finally {
                CodeMateForm.setLoading(submitBtn, false);
            }
        });
    })();
