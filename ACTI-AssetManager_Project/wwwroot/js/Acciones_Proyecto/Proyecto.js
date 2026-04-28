function abrirModalProyecto() {
    const modal = document.getElementById('modalProyecto');
    document.getElementById('formProyecto').reset();
    document.getElementById('idProyecto').value = "0";
    document.getElementById('tituloModal').innerText = "Nuevo Proyecto";

    modal.style.display = 'flex';
    setTimeout(() => modal.classList.add('is-active'), 10);
}

function cerrarModalProyecto() {
    const modal = document.getElementById('modalProyecto');
    modal.classList.remove('is-active');
    setTimeout(() => modal.style.display = 'none', 400);
}

// Para editar, también aplicamos la animación
function editarProyecto(id, nombre, desc) {
    const modal = document.getElementById('modalProyecto');
    document.getElementById('idProyecto').value = id;
    document.getElementById('nombreProyecto').value = nombre;
    document.getElementById('descripcion').value = desc;
    document.getElementById('tituloModal').innerText = "Editar Proyecto";

    modal.style.display = 'flex';
    setTimeout(() => modal.classList.add('is-active'), 10);
}

// Envío del formulario (Manteniendo tu lógica de jQuery)
$('#formProyecto').on('submit', function (e) {
    e.preventDefault();

    const id = $('#idProyecto').val();
    const url = id == "0" ? '/Proyecto/Registrar' : '/Proyecto/Actualizar';

    const formData = {
        IdProyecto: parseInt(id),
        NombreProyecto: $('#nombreProyecto').val(),
        Descripcion: $('#descripcion').val()
    };

    $.post(url, formData)
        .done(function (res) {
            if (res.success) {
                // El z-index de Swal ya lo arreglamos en el CSS anterior
                Swal.fire('¡Éxito!', 'El proyecto ha sido guardado.', 'success')
                    .then(() => location.reload());
            } else {
                Swal.fire('Error', 'No se pudo guardar el proyecto.', 'error');
            }
        })
        .fail(function () {
            Swal.fire('Error', 'Fallo en la comunicación con el servidor', 'error');
        });
});