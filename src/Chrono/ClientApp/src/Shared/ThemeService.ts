import {
  enable,
  disable,
  setFetchMethod,
} from 'darkreader';

const theme = {
  brightness: 100,
  contrast: 100,
  sepia: 0
}

export class ThemeService {
  static init() {
    setFetchMethod(window.fetch);
    
    if (this.isDarkModeEnabled())
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
    if (this.isDarkModeEnabled())
      this.disableDarkMode();
    else
      this.enableDarkMode();
  }
  
  static isDarkModeEnabled() {
    return localStorage.getItem('isDarkMode') === 'true';
  }
}