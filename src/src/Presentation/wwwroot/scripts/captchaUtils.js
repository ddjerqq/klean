function renderGoogleRecaptcha(recaptchaComponent, selector, siteKey) {
  return grecaptcha.render(selector, {
    'sitekey': siteKey,
    'callback': (response) => { recaptchaComponent.invokeMethodAsync('OnSuccess', response); },
    'expired-callback': () => { recaptchaComponent.invokeMethodAsync('OnExpired'); }
  });
}