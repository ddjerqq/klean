window.initFlowbite = () => {
  initFlowbite();
};

/**
 * get cookie
 * @param {string} name
 * @returns {string | null}
 */
window.getCookie = (name) =>
{
  const nameEQ = name + "=";
  const ca = document.cookie.split(';');
  for (let i = 0; i < ca.length; i++) {
    let c = ca[i];
    while (c.charAt(0) === ' ') c = c.substring(1, c.length);
    if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
  }
  return null;
}

/**
 * set cookie
 * @param {string} name
 * @param {string} value
 */
window.setCookie = (name, value) =>
{
  const expires = "; Max-Age=" + (7 * 24 * 60 * 60);
  const secure = "; Secure";
  const sameSite = "; SameSite=None";
  const path = "; Path=/"; // Cookie is accessible throughout the site

  const cookie = name + "=" + (value || "") +
    expires +
    secure +
    sameSite +
    path;

  console.log("setting cookie", cookie);

  document.cookie = cookie;
}

/**
 * delete cookie
 * @param {string} name
 */
window.delCookie = (name) =>
{
  document.cookie = name + "=; Max-Age=-99999999;";
}

/**
 * Get browser time zone
 * @returns {string}
 */
window.getBrowserTimeZone = () => {
  const options = Intl.DateTimeFormat().resolvedOptions();
  return options.timeZone;
};

/**
 * Get browser locale
 * @returns {string}
 */
window.getBrowserLocale = () => {
  if (navigator.languages !== undefined)
    return navigator.languages[0];

  return navigator.language;
}