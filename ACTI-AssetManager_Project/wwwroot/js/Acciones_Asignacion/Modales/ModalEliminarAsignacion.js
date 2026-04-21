let idAsignacionAEliminar = null;

window.confirmarEliminar = function(idAsignacion) {
    idAsignacionAEliminar = idAsignacion;
    console.log("Preparando para eliminar ID:", idAsignacion);
    
    const modal = document.getElementById('modalConfirmarEliminar');
    if (modal) {
        modal.style.display = 'flex';
    }
}

window.cerrarModalEliminar = function() {
    const modal = document.getElementById('modalConfirmarEliminar');
    if (modal) {
        modal.style.display = 'none';
    }
    idAsignacionAEliminar = null;
}

// Cerrar si hacen clic fuera del modal
window.addEventListener('click', function(e) {
    const modal = document.getElementById('modalConfirmarEliminar');
    if (e.target === modal) {
        cerrarModalEliminar();
    }
});

// Evento para el botón de "Sí, eliminar" (Por ahora solo log)
document.getElementById('btnConfirmarEliminarFinal')?.addEventListener('click', function() {
    if (idAsignacionAEliminar) {
        console.log("Eliminando asignación con ID:", idAsignacionAEliminar);
        // Aquí irá tu fetch más adelante
        cerrarModalEliminar();
    }
});