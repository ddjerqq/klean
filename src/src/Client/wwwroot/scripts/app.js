/**
 * get cookie
 * @param {string} name
 * @returns {string}
 */
export function getCookie(name) {
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
export function setCookie(name, value) {
  const expires = "; Max-Age=" + (7 * 24 * 60 * 60); // 7 days
  const secure = "; Secure"; // Cookie is only sent over HTTPS
  const sameSite = "; SameSite=None"; // Cross-site requests allowed
  const path = "; Path=/"; // Cookie is accessible throughout the site

  const cookie = name + "=" + (value || "") +
    expires +
    secure +
    sameSite +
    path;

  console.log(cookie);

  document.cookie = cookie;
}

/**
 * delete cookie
 * @param {string} name
 */
export function delCookie(name) {
  document.cookie = name + "=; Max-Age=-99999999;";
}

/**
 * Save as file
 * @param {string} filename
 * @param {string} bytesBase64
 */
export function saveAsFile(filename, bytesBase64) {
  const link = document.createElement("a");
  link.download = filename;
  link.href = `data:application/octet-stream;base64,${bytesBase64}`;
  document.body.appendChild(link); // Needed for Firefox
  link.click();
  document.body.removeChild(link);
}

/**
 * Get browser time zone
 * @returns {string}
 */
export function getBrowserTimeZone() {
  const options = Intl.DateTimeFormat().resolvedOptions();
  return options.timeZone;
}