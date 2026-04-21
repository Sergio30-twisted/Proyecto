var idAsignacionEdicionActual = null;

async function abrirModalEditarAsignacion(idAsignacion) {
    // 1. MOSTRAR PRIMERO para dar feedback inmediato al usuario
    const overlay = document.getElementById('modalAsignacion');
    if (overlay) {
        overlay.style.setProperty("display", "flex", "important");
    }

    try {
        console.log("Cargando ID:", idAsignacion);
        idAsignacionEdicionActual = idAsignacion;

        const response = await fetch(`/Asignacion/ObtenerInformacionAsignacion?id=${idAsignacion}`);
        const data = await response.json();
        console.log("Datos recibidos:", data);

        // 2. LIMPIEZA
        const contenedor = document.getElementById('contenedorChips');
        if (contenedor) contenedor.innerHTML = '';

        const form = document.getElementById('formAsignacion');
        if (form) form.reset();

        // 3. LLENADO DE CAMPOS BÁSICOS
        const inputProyecto = document.getElementById('proyecto');
        if (inputProyecto) inputProyecto.value = data.idProyecto || "";

        if (data.fechaInicio) {
            const fechaRango = document.getElementById('fechaRango');
            if (fechaRango) fechaRango.value = `${data.fechaInicio} a ${data.fechaFin || ''}`;

            document.getElementById('fechaInicio').value = data.fechaInicio;
            document.getElementById('fechaFin').value = data.fechaFin || "";
        }

        // 4. RECONSTRUCCIÓN DE USUARIOS Y SUS RECURSOS
        if (data.usuarios) {
            data.usuarios.forEach(user => {
                // Creamos la tarjeta (esta función ahora también llena el select interno)
                const tarjeta = crearTarjetaUsuarioDesdeData(user.idUsuario, user.nombre);

                // Si la tarjeta se creó y tiene recursos previos, los dibujamos como "pills"
                if (tarjeta && user.recursos) {
                    user.recursos.forEach(idRec => añadirRecursoATarjetaDesdeData(tarjeta, idRec));
                }
            });
        }

        // 5. CONFIGURACIÓN DEL BOTÓN GUARDAR
        const btnGuardar = document.getElementById('btnGuardarAsignacion');
        if (btnGuardar) {
            document.querySelector('.modal-title').innerText = "Editar Asignación de Recurso";
            btnGuardar.innerText = "Guardar Cambios";
            btnGuardar.dataset.idEditar = idAsignacion;
        }

    } catch (error) {
        console.error("Fallo visual en el llenado:", error);
    }
}

function crearTarjetaUsuarioDesdeData(id, nombre) {
    const contenedor = document.getElementById('contenedorChips');
    const template = document.getElementById('tempTarjetaUsuario');
    if (!template || !contenedor) return null;

    const clon = template.content.cloneNode(true);

    // Nombre e ID en el dataset
    const nombreElement = clon.querySelector('.nombre-usuario');
    if (nombreElement) nombreElement.innerText = nombre;

    const tarjetaDiv = clon.querySelector('.tarjeta-dinamica');
    if (tarjetaDiv) tarjetaDiv.dataset.idUsuario = id;

    // --- INCORPORACIÓN: Llenar el select interno de la tarjeta ---
    const selectInterno = clon.querySelector('.select-recurso-interno');
    const plantillaRecursos = document.getElementById('recursoPlantilla');

    if (selectInterno && plantillaRecursos) {
        // Copiamos las opciones del select oculto que viene de la base de datos
        Array.from(plantillaRecursos.options).forEach(opt => {
            const nuevaOpt = document.createElement('option');
            nuevaOpt.value = opt.value;
            nuevaOpt.text = opt.text;
            nuevaOpt.disabled = opt.disabled;
            selectInterno.appendChild(nuevaOpt);
        });
    }

    contenedor.appendChild(clon);

    // Actualizar contador visual de usuarios
    const spanContador = document.getElementById('contadorUsuarios');
    if (spanContador) spanContador.innerText = contenedor.querySelectorAll('.tarjeta-dinamica').length;

    return contenedor.lastElementChild;
}

function añadirRecursoATarjetaDesdeData(tarjeta, idRecurso) {
    const contenedorPills = tarjeta.querySelector('.contenedor-pills-recursos');
    if (!contenedorPills) return;

    // Buscar el nombre del recurso para mostrarlo en el Pill
    const selectRecursos = document.getElementById('recursoPlantilla');
    let nombreRecurso = "ID: " + idRecurso;

    if (selectRecursos) {
        const opcion = [...selectRecursos.options].find(o => o.value == idRecurso);
        if (opcion) nombreRecurso = opcion.text;
    }

    const pill = document.createElement('div');
    pill.className = 'recurso-pill';
    pill.innerHTML = `
        <span>${nombreRecurso}</span>
        <button type="button" class="btn-quitar-recurso" onclick="this.parentElement.remove()">×</button>
        <input type="hidden" name="recursos_${tarjeta.dataset.idUsuario}" value="${idRecurso}">
    `;
    contenedorPills.appendChild(pill);
}

// --- LOGICA PARA BOTONES "+" DENTRO DE LAS TARJETAS ---
document.addEventListener('click', function (e) {
    if (e.target && e.target.classList.contains('btn-añadir-recurso')) {
        const tarjeta = e.target.closest('.tarjeta-dinamica');
        const select = tarjeta.querySelector('.select-recurso-interno');
        const idRecurso = select.value;

        if (idRecurso) {
            añadirRecursoATarjetaDesdeData(tarjeta, idRecurso);
        }
    }
});