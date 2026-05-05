/**
 * Abre el modal de agregar usuarios
 */
function abrirRegistroUsuario() {
    const modal = document.getElementById('regUserModal');
    modal.style.display = 'flex';

    setTimeout(() => {
        modal.classList.add('is-active');
    }, 10);

    const dropdown = document.getElementById('userDropdown');
    if (dropdown) dropdown.classList.remove('show');
}

/**
 * Cierra el modal de agregar usuarios
 */
function cerrarRegistroUsuario() {
    const modal = document.getElementById('regUserModal');
    modal.classList.remove('is-active');

    setTimeout(() => {
        modal.style.display = 'none';
        document.getElementById('regUserForm').reset();
    }, 400);
}

/**
 * Procesa el guardado
 */
async function procesarGuardadoUsuario() {
    const form = document.getElementById('regUserForm');

    // 1. Validación de campos obligatorios
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }

    // 2. Empaquetar datos del formulario
    const formData = new FormData(form);

    try {
        // 3. Petición al Controller (UsuarioController / Acción Registrar)
        const response = await fetch('/Usuario/Registrar', {
            method: 'POST',
            body: formData
        });

        // Convertimos la respuesta a JSON
        const result = await response.json();

        // 4. Evaluar el resultado que viene del Controller
        if (result.success) {
            Swal.fire({
                title: '¡Usuario Creado!',
                text: result.message,
                icon: 'success',
                confirmButtonColor: '#2563eb'
            }).then(() => {
                cerrarRegistroUsuario(); // Cerramos el modal
                // Opcional: location.reload(); // Por si quieres refrescar una tabla de usuarios
            });
        } else {
            // Error de lógica (ej. el correo ya existe)
            Swal.fire({
                title: 'Atención',
                text: result.message,
                icon: 'warning',
                confirmButtonColor: '#f59e0b'
            });
        }

    } catch (error) {
        // Error de red o servidor caído
        console.error("Error en la petición:", error);
        Swal.fire('Error', 'No se pudo conectar con el servidor. Intente más tarde.', 'error');
    }
}

// Cerrar al hacer clic fuera
window.addEventListener('click', function (event) {
    const modal = document.getElementById('regUserModal');
    if (event.target == modal) {
        cerrarRegistroUsuario();
    }
});