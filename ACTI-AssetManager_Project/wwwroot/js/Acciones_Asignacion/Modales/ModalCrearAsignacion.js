document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById('modalAsignacion');
    const btnAbrir = document.getElementById('btnAbrirModalAsignacion');
    const btnCerrar = modal.querySelector('.btn-cerrarModalAsignar');
    const btnX = modal.querySelector('.btn-close');
    const form = document.getElementById('formAsignacion');

    // --- 1. INICIALIZAR CALENDARIO (Flatpickr) ---
    let fp = null;
    if (typeof flatpickr !== 'undefined') {
        fp = flatpickr("#fechaRango", {
            mode: "range",
            dateFormat: "Y-m-d",
            altInput: true,
            altFormat: "d/m/Y",
            locale: "es",
            allowInput: false,
            onClose: function (selectedDates, dateStr, instance) {
                if (selectedDates.length === 2) {
                    document.getElementById('fechaInicio').value = instance.formatDate(selectedDates[0], "Y-m-d");
                    document.getElementById('fechaFin').value = instance.formatDate(selectedDates[1], "Y-m-d");
                }
            }
        });
    }

    const mostrar = () => {
        modal.style.display = 'flex';
        document.querySelector('.modal-title').innerText = "Asignar Recurso a Proyecto";
        const btnGuardar = document.getElementById('btnGuardarAsignacion');
        if (btnGuardar) {
            btnGuardar.innerText = "Realizar Asignación";
            delete btnGuardar.dataset.idEditar;
        }
        setTimeout(() => modal.classList.add('show'), 10);
        document.body.style.overflow = 'hidden';
    };

    const ocultar = () => {
        modal.classList.remove('show');
        setTimeout(() => {
            modal.style.display = 'none';
            document.body.style.overflow = 'auto';
            form.reset();
            if (fp) fp.clear();
            document.getElementById('fechaInicio').value = "";
            document.getElementById('fechaFin').value = "";
            const contenedor = document.getElementById('contenedorChips');
            if (contenedor) contenedor.innerHTML = '';
            const spanContador = document.getElementById('contadorUsuarios');
            if (spanContador) spanContador.innerText = "0";
        }, 300);
    };

    btnAbrir?.addEventListener('click', mostrar);
    btnCerrar?.addEventListener('click', ocultar);
    btnX?.addEventListener('click', ocultar);

    modal.addEventListener('click', (e) => {
        if (e.target === modal) ocultar();
    });

    var mensaje = window.success;
    if (mensaje && mensaje.trim() !== "") {
        Swal.fire({
            icon: 'success',
            title: '¡Éxito!',
            text: mensaje,
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 4000,
            timerProgressBar: true,
            background: '#f0fff4',
            iconColor: '#28a745'
        });
    }
});

/* Validaciones del formulario corregidas */
document.getElementById('formAsignacion').addEventListener('submit', function (e) {
    let errores = [];

    // 1. Validar Fechas
    const fInicio = document.getElementById('fechaInicio');
    const fFin = document.getElementById('fechaFin');
    const inputVisualFecha = document.querySelector('.flatpickr-input') || document.getElementById('fechaRango');

    if (!fInicio.value || !fFin.value) {
        errores.push("El periodo de asignación es obligatorio.");
        if (inputVisualFecha) inputVisualFecha.style.border = "1px solid red";
    } else {
        if (inputVisualFecha) inputVisualFecha.style.border = "";
    }

    // 2. Validar que existan usuarios
    tarjetas.forEach((tarjeta) => {
        // Intentamos buscar por clase .recurso-pill O por cualquier input hidden de recursos
        // Esto asegura que si el chip visual existe, lo encuentre
        const pills = tarjeta.querySelectorAll('.recurso-pill');
        const inputs = tarjeta.querySelectorAll('input[name^="RecursosIds"]');

        // Si no encuentra pills NI inputs, entonces realmente está vacío
        if (pills.length === 0 && inputs.length === 0) {
            usuariosSinRecurso++;
            tarjeta.style.border = "2px solid red";
            tarjeta.style.backgroundColor = "#fff5f5"; // Fondo levemente rojo para notar el error
        } else {
            tarjeta.style.border = "1px solid #dee2e6";
            tarjeta.style.backgroundColor = "";
        }
    });

    if (usuariosSinRecurso > 0) {
        errores.push("Cada usuario debe tener al menos un recurso asignado.");
    }

    if (errores.length > 0) {
        e.preventDefault();
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                // Esto asegura que la alerta se renderice sobre el modal
                target: document.getElementById('modalAsignacion'),
                icon: 'error',
                title: 'Datos incompletos',
                html: `<ul style="text-align: left;">${errores.map(err => `<li>${err}</li>`).join('')}</ul>`,
                confirmButtonColor: '#d33'
            });
        } else {
            alert("Errores:\n" + errores.join("\n"));
        }
    }
});