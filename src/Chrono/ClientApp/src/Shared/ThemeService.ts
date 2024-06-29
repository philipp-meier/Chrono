import {
  enable,
  disable,
} from 'darkreader';

const theme = {
  brightness: 100,
  contrast: 100,
  sepia: 0
}

export class ThemeService {
  static init() {
    if (localStorage.getItem('isDarkMode') === 'true')
      enable(theme);
    else
      disable()
  }
  
  static disableDarkMode() {
    disable();
    localStorage.setItem('isDarkMode', 'false');
  }
  
  static enableDarkMode() {
    enable(theme);
    localStorage.setItem('isDarkMode', 'true');
  }
  
  static toggle() {
    if (localStorage.getItem('isDarkMode') === 'true')
      this.disableDarkMode();
    else
      this.enableDarkMode();
  }
}