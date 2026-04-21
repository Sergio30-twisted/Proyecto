(function () {
    'use strict';

    // Auto-dismiss de alertas de éxito
    const alertaExito = document.querySelector('.alert-exito');
    if (alertaExito) {
        setTimeout(() => {
            alertaExito.style.transition = 'opacity 0.5s ease';
            alertaExito.style.opacity = '0';
            setTimeout(() => alertaExito.remove(), 500);
        }, 4000);
    }

    // Filtro en vivo sobre la tabla
    const inputBuscar = document.querySelector('.filtro-input[name="filtroCodigo"]');
    if (inputBuscar) {
        inputBuscar.addEventListener('input', function () {
            const q = this.value.toLowerCase().trim();
            const filas = document.querySelectorAll('#tabla-inventario tbody tr');
            filas.forEach(fila => {
                const codigo = fila.cells[0]?.textContent.toLowerCase() ?? '';
                const responsable = fila.cells[5]?.textContent.toLowerCase() ?? '';
                const match = !q || codigo.includes(q) || responsable.includes(q);
                fila.style.display = match ? '' : 'none';
            });
        });
    }

    // Highlight de fila
    document.querySelectorAll('#tabla-inventario tbody tr').forEach(fila => {
        fila.addEventListener('mouseenter', () => fila.classList.add('fila-hover'));
        fila.addEventListener('mouseleave', () => fila.classList.remove('fila-hover'));
    });

    // Validación de fecha
    const fechaAdq = document.getElementById('FechaAdquisicion');
    const fechaVig = document.getElementById('Vigencia');
    const formCard = document.querySelector('form');

    if (formCard && fechaAdq && fechaVig) {
        formCard.addEventListener('submit', function (e) {
            // Validamos solo si son visibles (offsetParent !== null)
            if (fechaAdq.value && fechaVig.value && fechaVig.offsetParent !== null) {
                if (fechaVig.value < fechaAdq.value) {
                    e.preventDefault();
                    alert('La vigencia no puede ser anterior a la fecha de adquisición.');
                    fechaVig.focus();
                    return false;
                }
            }
        });
    }
})(); 

// Logica para ocultar la vigencia
(function () {
    'use strict';

    function gestionarVisibilidadVigencia() {
        const comboTipo = document.getElementById("IdTipoRecurso");
        const contenedorVigencia = document.getElementById("contenedor-vigencia");
        const inputVigencia = contenedorVigencia ? contenedorVigencia.querySelector('input') : null;

        if (!comboTipo || !contenedorVigencia) return;

        const selectedOption = comboTipo.options[comboTipo.selectedIndex];

        // Obtenemos la categoría desde el atributo data-categoria que pusimos en el HTML
        // 1 = Hardware, 2 = Software con Vigencia, 3 = Software sin Vigencia
        const idCategoria = selectedOption ? selectedOption.getAttribute("data-categoria") : null;

        // REGLA: Ocultar si es Hardware (1) o Software Perpetuo (3)
        if (idCategoria === "1" || idCategoria === "3") {
            contenedorVigencia.style.display = "none";
            if (inputVigencia) inputVigencia.value = ""; // Limpiamos para evitar basura en la DB
        }
        else {
            // Mostrar si es Software con Vigencia (2)
            contenedorVigencia.style.display = "";
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        const comboTipo = document.getElementById("IdTipoRecurso");
        if (comboTipo) {
            comboTipo.addEventListener("change", gestionarVisibilidadVigencia);
            // Ejecutar al cargar por si es una edición o recarga
            gestionarVisibilidadVigencia();
        }
    });
})();

// MODAL PARA ASIGNAR RESPONSABLE
const modalAsignar = document.getElementById('modal-AsignarResponsable');
const hdnIdAsignacion = document.getElementById('hdn-IdRecurso-Asig');
const selectResponsable = document.getElementById('select-UsuarioResponsable');

function AbrirModalAsignarResponsable(id) {
    if (!modalAsignar) return;
    hdnIdAsignacion.value = id;
    modalAsignar.style.display = 'flex';
    document.body.style.overflow = 'hidden';
}

function CerrarModalAsignar() {
    if (!modalAsignar) return;
    modalAsignar.style.display = 'none';
    document.body.style.overflow = 'auto';
    if (selectResponsable) selectResponsable.value = "";
}

window.onclick = function (event) {
    if (event.target == modalAsignar) {
        CerrarModalAsignar();
    }
}

function obtenerDatosDelResponsable() {
    // 1. Referencia al elemento select
    const select = document.getElementById("select-UsuarioResponsable");

    if (!select || select.selectedIndex === -1) {
        return null;
    }

    // 2. Obtener el ID (el atributo value="@res.Value")
    const idSeleccionado = select.value;

    // 3. Obtener el Nombre (el texto que está entre <option>...</option>)
    // Usamos selectedIndex para saber cuál está marcado y sacar su texto
    const nombreSeleccionado = select.options[select.selectedIndex].text;

    // Validación simple: si no seleccionó nada
    if (!idSeleccionado) {
        alert("Por favor, selecciona un responsable");
        return null;
    }

    // Retornamos un objeto con ambos datos
    return {
        id: idSeleccionado,
        nombre: nombreSeleccionado
    };
}

async function GuardarAsignacion() {
    try {
        const idRecurso = document.getElementById('hdn-IdRecurso-Asig')?.value;
        const idUsuario = document.getElementById('select-UsuarioResponsable')?.value;

        if (!idUsuario) {
            alert("Por favor, selecciona un usuario responsable.");
            return;
        }

        const datos = new FormData();
        datos.append('idRecurso', idRecurso);
        datos.append('idUsuario', idUsuario);

        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        if (token) datos.append('__RequestVerificationToken', token);

        const respuesta = await fetch('/Recursos/AsignarResponsable', {
            method: 'POST',
            body: datos
        });

        if (respuesta.ok) {
            // 1. Cerramos el modal visualmente
            CerrarModalAsignar();

            // 2. Recargamos la página para ver los cambios
            window.location.reload();
        } else {
            const errorTexto = await respuesta.text();
            alert("Error al asignar: " + errorTexto);
        }
    } catch (err) {
        alert("Ocurrió un error de conexión al intentar asignar el recurso.");
    }
}




// Función para abrir el modal (también global)
window.AbrirModalAsignarResponsable = function (id) {
    const input = document.getElementById('hdn-IdRecurso-Asig');
    const modal = document.getElementById('modal-AsignarResponsable');

    if (input && modal) {
        input.value = id;
        modal.style.display = 'flex';
    } else {
        console.error("No se encontró el input o el modal en el DOM");
    }
};

window.CerrarModalAsignar = function () {
    document.getElementById('modal-AsignarResponsable').style.display = 'none';
};

function CerrarModalAsignar() {
    document.getElementById('modal-AsignarResponsable').style.display = 'none';
}


// 1. ESTA ES LA FUNCIÓN QUE USAN TUS BOTONES (CHIPS)
function aplicarFiltroGeneral(idInput, valor) {
    console.log("Intentando filtrar: ", idInput, " con valor: ", valor); // Debug en consola

    const inputHidden = document.getElementById(idInput);
    const formulario = document.querySelector('.filtros-panel');

    if (inputHidden && formulario) {
        inputHidden.value = valor;
        formulario.submit(); // Al ser un GET, enviará filtroTipo y filtroEstado en la URL
    } else {
        console.error("No se encontró el input " + idInput + " o el formulario .filtros-panel");
    }
}

// 2. Estas funciones son por si aún las llamas desde algún otro lado, 
// pero internamente usan la lógica de arriba para no repetir.
function aplicarFiltroTipo(valor) {
    aplicarFiltroGeneral('hdn-filtroTipo', valor);
}

function aplicarFiltroEstado(valor) {
    aplicarFiltroGeneral('hdn-filtroEstado', valor);
}

// Función auxiliar para no repetir código
function enviarFormularioFiltros() {
    const formulario = document.querySelector('.filtros-panel');
    if (formulario) {
        formulario.submit();
    } else {
        console.error("No se encontró el formulario con la clase .filtros-panel");
    }
}

// --- LÓGICA PARA ELIMINACIÓN ---

let idRecursoAEliminar = null;

// Esta función se activa al hacer clic en el botón "Eliminar" de la tabla
window.abrirModalEliminar = function (id, codigo) {
    idRecursoAEliminar = id;

    // 1. Buscamos el modal y el texto donde mostraremos el código del recurso
    const modal = document.getElementById('modal-eliminar'); // Asegúrate que el ID coincida con tu HTML
    const spanCodigo = document.getElementById('codigo-recurso-eliminar');

    if (modal) {
        if (spanCodigo) spanCodigo.innerText = codigo;
        modal.style.display = 'flex'; // Mostramos el modal
        document.body.style.overflow = 'hidden'; // Bloqueamos scroll
    } else {
        console.error("No se encontró el modal de eliminación con ID 'modal-eliminar'");
    }
};

window.cerrarModalEliminar = function () {
    const modal = document.getElementById('modal-eliminar');
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = 'auto';
        idRecursoAEliminar = null;
    }
};

// Esta función se activa al presionar "Confirmar" dentro del modal
window.confirmarEliminacion = function () {
    if (idRecursoAEliminar) {
        // Buscamos el formulario específico de ese recurso
        const formulario = document.getElementById('form-eliminar-' + idRecursoAEliminar);

        if (formulario) {
            formulario.submit(); // ¡Aquí es donde realmente se borra!
        } else {
            console.error("No se encontró el formulario: form-eliminar-" + idRecursoAEliminar);
        }
    }
};


/*Este codigo de abajo sirve para que si elejimos crear un recurso sin capacidad de uso, se bloquea
el input donde ingresamos la capacidad, de lo contrario es mantiene activo,*/

// Usamos una función autoejecutable para proteger el código
(function () {
    const inicializarCapacidad = () => {
        const chk = document.getElementById("chkNoCapacidad");
        const input = document.getElementById("inputCapacidad");

        // Solo actuamos si ambos elementos existen en la página actual
        if (chk && input) {

            // Definimos la lógica
            const actualizarEstado = () => {
                input.disabled = chk.checked;
                if (chk.checked) {
                    input.value = "";
                    input.style.backgroundColor = "#f2f2f2";
                } else {
                    input.style.backgroundColor = "#ffffff";
                }
            };

            // ASIGNAMOS EL EVENTO (Aquí es donde ocurre la magia)
            chk.addEventListener("change", actualizarEstado);

            // EJECUTAMOS AL CARGAR (Para que empiece bloqueado si el check está activo)
            actualizarEstado();
        }
    };

    // Intentamos ejecutarlo de inmediato
    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", inicializarCapacidad);
    } else {
        inicializarCapacidad();
    }
})();