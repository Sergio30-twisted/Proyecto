(function () {
    'use strict';

    // Auto-dismiss alertas de éxito
    const alertaExito = document.querySelector('.alert-exito');
    if (alertaExito) {
        setTimeout(() => {
            alertaExito.style.transition = 'opacity 0.5s ease';
            alertaExito.style.opacity = '0';
            setTimeout(() => alertaExito.remove(), 500);
        }, 4000);
    }

    // Filtro en vivo en la tabla
    const inputBuscar = document.querySelector('.filtro-input[name="filtroBusqueda"]');
    if (inputBuscar) {
        inputBuscar.addEventListener('input', function () {
            const q = this.value.toLowerCase().trim();
            document.querySelectorAll('#tabla-licencias tbody tr').forEach(fila => {
                const codigo = fila.cells[0]?.textContent.toLowerCase() ?? '';
                fila.style.display = !q || codigo.includes(q) ? '' : 'none';
            });
        });
    }

    // Preview de "Disponibles" en el formulario
    const inputTotal = document.getElementById('CantidadTotal');
    const inputEnUso = document.getElementById('CantidadEnUso');
    const preview = document.getElementById('disponibles-preview');

    function actualizarDisponibles() {
        if (!inputTotal || !inputEnUso || !preview) return;
        const total = parseInt(inputTotal.value) || 0;
        const enUso = parseInt(inputEnUso.value) || 0;
        const disp = total - enUso;
        preview.textContent = disp >= 0 ? disp : '⚠️ En uso supera el total';
        preview.style.color = disp < 0 ? '#dc2626' : '#0f172a';
    }

    if (inputTotal) inputTotal.addEventListener('input', actualizarDisponibles);
    if (inputEnUso) inputEnUso.addEventListener('input', actualizarDisponibles);
    actualizarDisponibles();

    // Validación: en uso ≤ total
    const form = document.querySelector('.form-card form');
    if (form) {
        form.addEventListener('submit', function (e) {
            const total = parseInt(inputTotal?.value) || 0;
            const enUso = parseInt(inputEnUso?.value) || 0;
            if (enUso > total) {
                e.preventDefault();
                abrirModalValidacion('La cantidad en uso no puede superar la cantidad total.');
            }
        });
    }

})();

// Modal de validación
function abrirModalValidacion(mensaje) {
    document.getElementById('modal-validacion-mensaje').textContent = mensaje;
    document.getElementById('modal-validacion').style.display = 'flex';
}

function cerrarModalValidacion() {
    document.getElementById('modal-validacion').style.display = 'none';
}

//------------

let formEliminarActivo = null;

function abrirModalEliminar(btn) {
    const form = btn.closest('form');
    const nombre = form.dataset.nombre;
    formEliminarActivo = form;
    document.getElementById('modal-nombre-licencia').textContent = nombre;
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
