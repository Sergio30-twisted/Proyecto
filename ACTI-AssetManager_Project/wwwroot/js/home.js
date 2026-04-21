function toggleMenu(event) {
    // Esto es vital: evita que el clic "atraviese" el div y llegue al fondo de la página
    event.stopPropagation();

    const dropdown = document.getElementById("userDropdown");

    // Verificamos si el elemento existe antes de actuar
    if (dropdown) {
        dropdown.classList.toggle("show");
        console.log("Menú clickeado, clase 'show' aplicada:", dropdown.classList.contains("show"));
    }
}

// Cerrar si haces clic afuera
document.addEventListener("click", function (event) {
    const dropdown = document.getElementById("userDropdown");
    const container = document.querySelector(".user-menu-container");

    if (dropdown && !container.contains(event.target)) {
        dropdown.classList.remove("show");
    }
});