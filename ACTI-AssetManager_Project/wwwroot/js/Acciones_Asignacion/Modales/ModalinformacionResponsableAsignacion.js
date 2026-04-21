window.verDetalles = async function (idAsignacion) {
    const contenedor = document.querySelector('.contenedor-infoUsuario');
    if (!contenedor) return;

    // Limpiamos y mostramos un loader visual
    contenedor.innerHTML = '<p class="text-center">Cargando...</p>';

    // Mostramos el modal de inmediato para dar feedback al usuario
    // En lugar de solo .style.display = 'block';
    const modal = document.getElementById('modalVerResponsables');
    modal.style.display = 'flex'; // Usamos flex para que el contenido se centre
    modal.style.zIndex = '1050';

    try {
        const response = await fetch(`/Asignacion/ObtenerDetallesResponsables?idAsignacion=${idAsignacion}`);

        if (!response.ok) throw new Error("Error en el servidor");

        const datos = await response.json();
        contenedor.innerHTML = ''; // Quitamos el "Cargando..."

        if (datos.length === 0) {
            contenedor.innerHTML = '<p class="text-muted">No hay responsables asignados.</p>';
            return;
        }

        datos.forEach(item => {
            const html = `
                <div class="FotoUser-Nombre-Recurso d-flex align-items-center mb-3">
                    <div class="foto me-3">
                        <img src="${item.fotoUrl}" class="rounded-circle" width="50" height="50">
                    </div>
                    <div class="nombre me-3">
                        <p class="valor mb-0 fw-bold">${item.nombreUsuario}</p>
                    </div>
                    <div class="recurso">
                        <span class="badge-recurso-premium">${item.nombreRecurso}</span>
                    </div>
                </div>`;
            contenedor.insertAdjacentHTML('beforeend', html);
        });

    } catch (error) {
        contenedor.innerHTML = '<p class="text-danger">Error al cargar los datos.</p>';
        console.error("Error:", error);
    }
};

document.addEventListener('click', function (e) {
    // Buscamos si el clic o su padre es el botón
    const btn = e.target.closest('.btn-ver-responsables');

    if (btn) {
        e.preventDefault(); // Evitamos cualquier acción por defecto
        const id = btn.getAttribute('data-id');
        console.log("Botón presionado, ID detectado:", id);

        // Llamamos explícitamente a la función global
        window.verDetalles(id);
    }
});

// Función universal para cerrar
function cerrarModalResponsables() {
    const modal = document.getElementById('modalVerResponsables');
    if (modal) {
        modal.style.display = 'none';
    }
}

// Escuchamos el clic en CUALQUIER botón de cerrar dentro del modal
document.getElementById('modalVerResponsables').addEventListener('click', function (e) {
    // Si clickean la "X", el botón "Cerrar" del footer, o el fondo oscuro
    if (e.target.classList.contains('btn-close') ||
        e.target.id === 'btnCerrarFooterVer' ||
        e.target === this) {
        cerrarModalResponsables();
    }
});