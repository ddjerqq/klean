window.initFlowbite = () => {
  initFlowbite();
};

/**
 * Get browser time zone
 * @returns {string}
 */
window.getBrowserTimeZone = () => {
  const options = Intl.DateTimeFormat().resolvedOptions();
  return options.timeZone;
};