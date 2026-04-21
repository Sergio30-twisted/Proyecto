let formEliminarActivo = null;

function abrirModalEliminar(btn) {
    const form = btn.closest('form');
    const nombre = form.dataset.nombre;
    formEliminarActivo = form;
    document.getElementById('modal-nombre-recurso').textContent = nombre;
    document.getElementById('modal-eliminar').style.display = 'flex';
}

function cerrarModalEliminar() {
    document.getElementById('modal-eliminar').style.display = 'none';
    formEliminarActivo = null;
}

function confirmarEliminar() {
    if (formEliminarActivo) formEliminarActivo.submit();
}

// Cerrar al hacer clic fuera del modal
document.getElementById('modal-eliminar').addEventListener('click', function (e) {
    if (e.target === this) cerrarModalEliminar();
});