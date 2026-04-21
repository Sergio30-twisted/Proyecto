let idAsignacionAEliminar = null;

window.confirmarEliminar = function (idAsignacion) {
    idAsignacionAEliminar = idAsignacion;
    const modal = document.getElementById('modalConfirmarEliminar');
    if (modal) modal.style.display = 'flex';
}

window.cerrarModalEliminar = function () {
    const modal = document.getElementById('modalConfirmarEliminar');
    if (modal) modal.style.display = 'none';
    idAsignacionAEliminar = null;
}

// Función para mostrar el recuadro "bonito"
function mostrarToast(mensaje) {
    const container = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = 'toast-custom';
    toast.innerHTML = `
        <i class="bi bi-check-circle-fill"></i>
        <div class="mensaje">${mensaje}</div>
    `;
    container.appendChild(toast);

    // Eliminar del DOM después de que termine la animación
    setTimeout(() => {
        toast.remove();
    }, 4000);
}

document.getElementById('btnConfirmarEliminarFinal')?.addEventListener('click', async function (e) {
    e.preventDefault();
    if (!idAsignacionAEliminar) return;

    this.innerText = "Eliminando...";
    this.disabled = true;

    try {
        const response = await fetch(`/Asignacion/EliminarLogica?id=${idAsignacionAEliminar}`, {
            method: 'POST'
        });

        const data = await response.json();

        if (data.success) {
            cerrarModalEliminar();

            // 1. Mostramos el recuadro bonito
            mostrarToast("La asignación se eliminó correctamente");

            // 2. Esperamos un poco para que el usuario vea el mensaje antes de recargar
            setTimeout(() => {
                window.location.reload();
            }, 1500);

        } else {
            alert("Error: " + data.message);
            this.innerText = "Sí, eliminar";
            this.disabled = false;
        }
    } catch (error) {
        console.error("Error:", error);
        this.innerText = "Sí, eliminar";
        this.disabled = false;
    }
});