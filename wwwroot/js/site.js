document.getElementById("date").innerText = new Date().getFullYear().toString();

document.addEventListener('DOMContentLoaded', function() {
  console.log('DOM loaded, initializing components...');
  
  // Custom sidenav implementation
  const sidenav = document.getElementById('slide-out');
  const sidenavTrigger = document.querySelector('.sidenav-trigger');
  const sidenavOverlay = document.querySelector('.sidenav-overlay');
  let sidenavOpen = false;
  
  function openSidenav() {
    if (sidenav && sidenavOverlay) {
      sidenavOpen = true;
      sidenav.classList.add('open');
      sidenavOverlay.classList.add('open');
      document.body.style.overflow = 'hidden';
    }
  }
  
  function closeSidenav() {
    if (sidenav && sidenavOverlay) {
      sidenavOpen = false;
      sidenav.classList.remove('open');
      sidenavOverlay.classList.remove('open');
      document.body.style.overflow = '';
    }
  }
  
  if (sidenavTrigger) {
    sidenavTrigger.addEventListener('click', function(e) {
      e.preventDefault();
      e.stopPropagation();
      if (sidenavOpen) {
        closeSidenav();
      } else {
        openSidenav();
      }
    });
  }
  
  if (sidenavOverlay) {
    sidenavOverlay.addEventListener('click', function(e) {
      closeSidenav();
    });
  }
  
  // Close sidenav when clicking a link inside it
  if (sidenav) {
    const sidenavLinks = sidenav.querySelectorAll('a');
    sidenavLinks.forEach(link => {
      link.addEventListener('click', function() {
        closeSidenav();
      });
    });
  }
  
  // App menu dropdown handling
  const appMenuDropdown = document.getElementById('app-menu-dropdown');
  const appMenuTrigger = document.getElementById('app-menu-trigger');
  
  if (appMenuTrigger && appMenuDropdown) {
    appMenuTrigger.addEventListener('click', function(e) {
      e.preventDefault();
      e.stopPropagation();
      appMenuDropdown.classList.toggle('open');
    });
  }
  
  // Close app menu when clicking outside
  document.addEventListener('click', function(e) {
    const target = e.target;
    if (!target.closest('.app-menu-container')) {
      if (appMenuDropdown) {
        appMenuDropdown.classList.remove('open');
      }
    }
  });
});
