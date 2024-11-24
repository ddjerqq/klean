window.toggleTheme = () => {
  const theme = localStorage.getItem('theme');
  localStorage.setItem('theme', theme === 'dark' ? 'light' : 'dark');
  document.documentElement.classList.toggle('dark');
};

window.addEventListener('load', () => {
  const theme = localStorage.getItem('theme');
  if (theme === 'dark' || (!('theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
    document.documentElement.classList.add('dark');
  }
});

window.dropdown = {
  show: async function(buttonRef, dotNetRef) {
    const button = await buttonRef.getBoundingClientRect();
    const dropdown = document.createElement('div');

    // Style dropdown container
    dropdown.style.position = 'absolute';
    dropdown.style.left = `${button.left}px`;
    dropdown.style.top = `${button.bottom}px`;
    dropdown.style.width = `${button.width}px`;
    dropdown.style.zIndex = 1000;
    dropdown.style.backgroundColor = 'white';
    dropdown.style.border = '1px solid #ccc';
    dropdown.style.boxShadow = '0px 4px 6px rgba(0,0,0,0.1)';
    dropdown.style.padding = '5px';
    dropdown.className = 'dropdown-menu';

    // Prevent scrolling
    document.body.style.overflow = 'hidden';

    // Add options
    const options = ['Option 1', 'Option 2']; // Replace with your logic
    options.forEach((option, index) => {
      const item = document.createElement('div');
      item.textContent = option;
      item.style.padding = '5px';
      item.style.cursor = 'pointer';
      item.addEventListener('click', () => {
        dotNetRef.invokeMethodAsync('SelectOption', `${index + 1}`);
        document.body.removeChild(dropdown);
        document.body.style.overflow = '';
      });
      dropdown.appendChild(item);
    });

    // Append to body
    document.body.appendChild(dropdown);
  },
  hide: function() {
    const dropdown = document.querySelector('.dropdown-menu');
    if (dropdown) {
      document.body.removeChild(dropdown);
    }
    // Restore scrolling
    document.body.style.overflow = '';
  },
};