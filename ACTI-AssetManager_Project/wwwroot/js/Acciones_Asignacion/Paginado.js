$(document).ready(function () {
    // Inicializamos la tabla
    const tablaAsignaciones = $('#tablaAsignaciones').DataTable({
        "destroy": true, // Permite re-inicializar si cambian los datos dinámicamente
        "pageLength": 5,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json",
            "emptyTable": "No hay asignaciones activas registradas" // Mensaje elegante cuando está vacío
        },
        "columnDefs": [
            { "orderable": false, "targets": [4, 5] } // Columnas de "Responsables" y "Acciones"
        ],
        "dom": 'rt<"d-flex justify-content-between align-items-center mt-3 px-2"ip>',
    });

    // Escuchamos el input de búsqueda
    $('#inputBusquedaGlobal').on('input', function () {
        const valorBusqueda = $(this).val();
        // Busca en la columna 0 (Proyecto)
        tablaAsignaciones.column(0).search(valorBusqueda).draw();
    });
});