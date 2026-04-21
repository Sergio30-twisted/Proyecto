async function confirmarEliminarTipo() {
    // 1. Obtener el ID y el Nombre para la confirmación
    const id = document.getElementById("editIdTipo").value;
    const nombre = document.getElementById("editNombreInput").value;

    if (!id) {
        alert("No se pudo obtener el ID del tipo a eliminar.");
        return;
    }

    // 2. Pedir confirmación al usuario
    const mensaje = `¿Estás seguro de que deseas eliminar el tipo: "${nombre}"?\n\nEsta acción no se puede deshacer.`;

    if (confirm(mensaje)) {
        try {
            // 3. Enviar la petición al Controller (Endpoint: /Recursos/EliminarTipoRecurso)
            const response = await fetch(`/Recursos/EliminarTipoRecurso?id=${id}`, {
                method: 'POST', // Usamos POST para mayor compatibilidad
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            //Aquí esperamos un resultado y mostramos un alert
            const resultado = await response.json();

            if (resultado.success) {
                alert("Eliminado con éxito.");
                cerrarModalEdicion(); // Llamamos a la función que ya tenemos
                location.reload();    // Recargamos la página para ver los cambios en la tabla
            } else {
                // Si el service detectó que está en uso, mostrará el mensaje de error
                alert("Error: " + resultado.message);
            }
        } catch (error) {
            console.error("Error al eliminar:", error);
            alert("Ocurrió un error en la comunicación con el servidor.");
        }
    }
}