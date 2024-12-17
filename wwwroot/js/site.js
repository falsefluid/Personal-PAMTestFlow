// Utility to set a cookie
function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000)); // Expiration time in days
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

// Utility to get a cookie
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

// Function to toggle dark/light mode
function toggleDarkLight() {
    var body = document.getElementById("body");
    var currentClass = body.className;
    
    // Toggle between dark and light mode
    var newClass = currentClass == "dark-mode" ? "light-mode" : "dark-mode";
    body.className = newClass;

    // Save the theme preference to cookies (with a 30-day expiration)
    setCookie("theme", newClass, 30);
}




// On page load, check for the saved theme cookie and apply it
window.addEventListener('DOMContentLoaded', (event) => {
    var savedTheme = getCookie("theme");
    
    // If a theme is saved in the cookie, apply it immediately
    if (savedTheme) {
        document.getElementById("body").className = savedTheme;
    }
});



document.getElementById("startTestForm").addEventListener("submit", function (event) {
    event.preventDefault();
    console.log("Start Test button clicked");
    fetch("/TestPage?handler=RunTest", { // Correctly pointing to the handler
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "RequestVerificationToken": token // Attach the anti-forgery token here
        }
    }).then(response => {
        if (response.ok) {
            console.log("Playwright test started");
        } else {
            console.error("Failed to start Playwright test");
        }
    }).catch(error => {
        console.error("Error starting Playwright test:", error);
    });
});

