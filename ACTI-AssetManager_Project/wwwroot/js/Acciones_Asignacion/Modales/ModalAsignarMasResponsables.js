// --- VARIABLES GLOBALES ---
let paginaActual = 0;
const usuariosPorPagina = 4;

// --- FUNCIÓN PRINCIPAL: AGREGAR USUARIO ---
function agregarTarjetaUsuario() {
    const selectRes = document.getElementById('selectResponsable');

    // 1. VALIDACIÓN: ¿Seleccionó algo?
    if (!selectRes.value || selectRes.selectedIndex === -1) {
        // Si usas SweetAlert2
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'warning',
                title: 'Campo requerido',
                text: 'Por favor, elige un usuario antes de añadirlo.',
                confirmButtonColor: '#0d6efd'
            });
        } else {
            alert("Por favor, elige un usuario.");
        }
        return; // Detenemos la función
    }

    // Ahora que estamos seguros de que hay selección, leemos los datos
    const uId = selectRes.value;
    const uNombre = selectRes.options[selectRes.selectedIndex].text;

    // 2. VALIDACIÓN: ¿Ya existe en la lista?
    if (document.getElementById('card-' + uId)) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'info',
                text: 'Este usuario ya ha sido añadido.'
            });
        }
        return;
    }

    // 3. Clonar el template
    const template = document.getElementById('tempTarjetaUsuario');
    const clon = template.content.cloneNode(true);

    // 4. Configurar datos de la tarjeta
    const contenedorCard = clon.querySelector('.tarjeta-dinamica');
    contenedorCard.id = 'card-' + uId;
    clon.querySelector('.nombre-usuario').textContent = uNombre;

    // 5. Llenar el select interno con los recursos de la plantilla
    const selectInterno = clon.querySelector('.select-recurso-interno');
    selectInterno.innerHTML = document.getElementById('recursoPlantilla').innerHTML;

    // 6. Configurar botón de eliminar tarjeta (Llama a la función global)
    clon.querySelector('.btn-close-card').onclick = () => eliminarTarjeta(uId);

    // 7. Configurar botón de añadir recurso (Dentro de la tarjeta)
    const btnAsignar = clon.querySelector('.btn-añadir-recurso') || clon.querySelector('.btn-agregar-recurso');

    btnAsignar.onclick = function () {
        // Validación interna para los recursos de esta tarjeta
        if (!selectInterno.value || selectInterno.selectedIndex === -1) {
            alert("Elige un recurso primero.");
            return;
        }

        const rId = selectInterno.value;
        const rNombre = selectInterno.options[selectInterno.selectedIndex].text;

        const listaPills = contenedorCard.querySelector('.contenedor-pills-recursos');
        const chipId = `chip-${uId}-${rId}`;

        if (document.getElementById(chipId)) return;

        // Crear el Pill/Chip
        const pill = document.createElement('span');
        pill.id = chipId;
        pill.className = 'badge badge-recurso-premium d-flex align-items-center';
        pill.innerHTML = `
            ${rNombre} 
            <span class="ms-2 text-danger" style="cursor:pointer" onclick="document.getElementById('${chipId}').remove()">&times;</span>
            <input type="hidden" name="UsuariosSeleccionadosIds" value="${uId}">
            <input type="hidden" name="RecursosSeleccionadosIds" value="${rId}">
        `;
        listaPills.appendChild(pill);

        // Resetear select interno
        selectInterno.value = "";
    };

    // 8. Inyectar en el DOM
    document.getElementById('contenedorChips').appendChild(clon);

    // 9. Actualizar UI
    if (typeof actualizarContador === "function") actualizarContador();
    if (typeof actualizarUIUsuarios === "function") actualizarUIUsuarios();

    // 10. Resetear select principal
    selectRes.value = "";
}


// --- FUNCIÓN: ELIMINAR TARJETA ---
function eliminarTarjeta(id) {
    const tarjeta = document.getElementById('card-' + id);
    if (tarjeta) {
        tarjeta.remove();
        actualizarContador();
        actualizarUIUsuarios();
    }
}

// --- FUNCIÓN: ACTUALIZAR CONTADOR ---
function actualizarContador() {
    const total = document.querySelectorAll('#contenedorChips .tarjeta-dinamica').length;
    const elContador = document.getElementById('contadorUsuarios');
    if (elContador) {
        elContador.innerText = total;
    }
}

// --- FUNCIÓN: CARRUSEL / PAGINACIÓN ---
function actualizarUIUsuarios() {
    const contenedor = document.getElementById('contenedorChips');
    const tarjetas = contenedor.querySelectorAll('.tarjeta-dinamica');
    const totalUsuarios = tarjetas.length;

    const totalPaginas = Math.ceil(totalUsuarios / usuariosPorPagina) || 1;

    if (paginaActual >= totalPaginas) paginaActual = totalPaginas - 1;
    if (paginaActual < 0) paginaActual = 0;

    tarjetas.forEach((tarjeta, index) => {
        const inicio = paginaActual * usuariosPorPagina;
        const fin = inicio + usuariosPorPagina;
        tarjeta.style.display = (index >= inicio && index < fin) ? "block" : "none";
    });

    // Actualizar botones e indicador
    const indicador = document.getElementById('indicadorPaginas');
    const btnPrev = document.getElementById('prevBtn');
    const btnNext = document.getElementById('nextBtn');

    if (indicador) indicador.innerText = `Página ${paginaActual + 1} de ${totalPaginas}`;
    if (btnPrev) btnPrev.disabled = (paginaActual === 0);
    if (btnNext) btnNext.disabled = (paginaActual >= totalPaginas - 1);
}

function moverCarrusel(direccion) {
    paginaActual += direccion;
    actualizarUIUsuarios();
}

// --- CONFIGURACIÓN LITEPICKER ---
document.addEventListener('DOMContentLoaded', function () {
    const elRango = document.getElementById('fechaRango');
    if (elRango) {
        const picker = new Litepicker({
            element: elRango,
            singleMode: false,
            numberOfMonths: 2,
            numberOfColumns: 2,
            format: 'DD/MM/YYYY',
            setup: (picker) => {
                picker.on('selected', (date1, date2) => {
                    document.getElementById('fechaInicio').value = date1.format('YYYY-MM-DD');
                    document.getElementById('fechaFin').value = date2.format('YYYY-MM-DD');
                });
            },
        });
    }
});