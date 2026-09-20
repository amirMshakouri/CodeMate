        const CodeMateApi = (() => {
            const SESSION_KEY = "codemate.session";

            function getSession() {
                try {
                    const raw = localStorage.getItem(SESSION_KEY);
                    return raw ? JSON.parse(raw) : null;
                } catch {
                    return null;
                }
            }

            function setSession(session) {
                localStorage.setItem(SESSION_KEY, JSON.stringify(session));
            }

            function clearSession() {
                localStorage.removeItem(SESSION_KEY);
            }

            function isAuthenticated() {
                const s = getSession();
                if (!s || !s.token) return false;
                if (s.expiration && new Date(s.expiration).getTime() <= Date.now()) {
                    clearSession();
                    return false;
                }
                return true;
            }

            function getClaims() {
                const s = getSession();
                if (!s || !s.token) return null;
                try {
                    let payload = s.token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
                    while (payload.length % 4) payload += "=";
                    const json = decodeURIComponent(
                        atob(payload)
                            .split("")
                            .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
                            .join("")
                    );
                    const raw = JSON.parse(json);
                    const NS = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/";
                    return {
                        id: raw[NS + "nameidentifier"],
                        userName: raw[NS + "name"],
                        email: raw[NS + "emailaddress"],
                        role: raw["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
                    };
                } catch {
                    return null;
                }
            }

            // فقط برای نمایش/مخفی‌کردن UI؛ کنترل واقعی دسترسی سمت بک‌اند انجام می‌شه
            function isAdmin() {
                const c = getClaims();
                return !!c && c.role === "Admin";
            }

            async function request(path, { method = "GET", body, auth = false } = {}) {
                const headers = { "Content-Type": "application/json" };

                if (auth) {
                    const session = getSession();
                    if (session && session.token) {
                        headers["Authorization"] = `Bearer ${session.token}`;
                    }
                }

                let response;
                try {
                    response = await fetch(`${window.CODEMATE_API_BASE}${path}`, {
                        method,
                        headers,
                        body: body !== undefined ? JSON.stringify(body) : undefined,
                    });
                } catch (networkErr) {
                    throw {
                        message:
                            "به API وصل نشد. مطمئن شو بک‌اند اجراست و آدرس داخل config.js درسته.",
                        errors: null,
                        networkError: true,
                    };
                }

                if (response.status === 401 && auth) {
                    clearSession();
                    window.location.href = "login.html";
                    return new Promise(() => { });
                }

                if (response.status === 204) {
                    return null;
                }

                let data = null;
                const text = await response.text();
                if (text) {
                    try {
                        data = JSON.parse(text);
                    } catch {
                        data = null;
                    }
                }

                if (!response.ok) {
                    // بک‌اند فیلدها رو با حرف بزرگ می‌فرسته (Message / Errors)؛ هر دو حالت خونده می‌شه
                    const errors = (data && (data.errors || data.Errors)) || null;
                    const firstError = errors ? Object.values(errors).flat()[0] : null;
                    throw {
                        message:
                            firstError ||
                            (data && (data.message || data.Message)) ||
                            `درخواست ناموفق بود (${response.status}).`,
                        errors,
                        status: response.status,
                    };
                }

                return data;
            }

            return {
                getSession,
                setSession,
                clearSession,
                isAuthenticated,
                getClaims,
                isAdmin,
                get: (path, opts) => request(path, { ...opts, method: "GET" }),
                post: (path, body, opts) => request(path, { ...opts, method: "POST", body }),
                put: (path, body, opts) => request(path, { ...opts, method: "PUT", body }),
                patch: (path, body, opts) => request(path, { ...opts, method: "PATCH", body }),
                del: (path, opts) => request(path, { ...opts, method: "DELETE" }),
            };
        })();
