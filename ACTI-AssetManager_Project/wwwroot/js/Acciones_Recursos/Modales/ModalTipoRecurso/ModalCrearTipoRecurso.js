document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById("modalCrearTipoRecurso");
    const btnAbrir = document.querySelector(".btn-modalCrearTipoRecurso");

    // Usamos selectores por clase para los botones de cierre
    const btnCerrarX = document.querySelector(".btn-cerrar-modal");
    const btnCancelar = document.querySelector(".btn-cancelar-modal");

    // Función Abrir
    if (btnAbrir) {
        btnAbrir.onclick = function () {
            modal.style.display = "block";
        }
    }

    // Función Cerrar
    const cerrar = () => {
        modal.style.display = "none";
    };

    if (btnCerrarX) btnCerrarX.onclick = cerrar;
    if (btnCancelar) btnCancelar.onclick = cerrar;

    // Cerrar al hacer clic fuera de la caja blanca
    window.onclick = function (event) {
        if (event.target == modal) {
            cerrar();
        }
    }
});