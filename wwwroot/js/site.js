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
  
  // Manual dropdown handling
  const appDropdown = document.getElementById('app-dropdown');
  const appTrigger = document.getElementById('app-drop-link');
  
  console.log('Dropdown elements:', { appDropdown, appTrigger });
  
  let showAppMenu = false;
  
  // Add click handlers
  if (appTrigger && appDropdown) {
    appTrigger.addEventListener('click', function(e) {
      e.preventDefault();
      e.stopPropagation();
      console.log('App trigger clicked, showAppMenu:', showAppMenu);
      
      showAppMenu = !showAppMenu;
      
      if (showAppMenu) {
        console.log('Opening app dropdown');
        appDropdown.style.setProperty('display', 'block', 'important');
        requestAnimationFrame(() => {
          appDropdown.style.setProperty('opacity', '1', 'important');
          appDropdown.style.setProperty('transform', 'scaleY(1)', 'important');
        });
      } else {
        console.log('Closing app dropdown');
        appDropdown.style.setProperty('opacity', '0', 'important');
        appDropdown.style.setProperty('transform', 'scaleY(0)', 'important');
        setTimeout(() => {
          appDropdown.style.setProperty('display', 'none', 'important');
        }, 300);
      }
    });
  }
  
  // Close dropdowns when clicking outside
  document.addEventListener('click', function(e) {
    const target = e.target;
    if (!target.closest('.appdropdown') && !target.closest('.dropdown-trigger')) {
      console.log('Clicked outside, closing dropdowns');
      showAppMenu = false;
      
      if (appDropdown) {
        appDropdown.style.setProperty('opacity', '0', 'important');
        appDropdown.style.setProperty('transform', 'scaleY(0)', 'important');
        setTimeout(() => {
          appDropdown.style.setProperty('display', 'none', 'important');
        }, 300);
      }
    }
  });
});
