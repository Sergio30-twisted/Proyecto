/* ============================================================
   1. CONTROL DEL OVERLAY DE EDICIÓN (EL FOCO)
   ============================================================ */
document.addEventListener("DOMContentLoaded", function () {
    const btnOpciones = document.getElementById("btnModoEdicion");
    const overlay = document.getElementById("overlayEdicion");
    const chipsEstados = document.querySelectorAll(".chip-estado-foco");

    if (btnOpciones && overlay) {
        btnOpciones.addEventListener("click", function () {
            const estaEncendido = overlay.style.display === "block";

            if (!estaEncendido) {
                overlay.style.display = "block";
                chipsEstados.forEach(chip => chip.classList.add("foco-activo"));
                btnOpciones.innerHTML = "✖";
            } else {
                apagarFoco();
            }
        });

        overlay.addEventListener("click", apagarFoco);
    }

    function apagarFoco() {
        if (overlay) overlay.style.display = "none";
        chipsEstados.forEach(chip => chip.classList.remove("foco-activo"));
        if (btnOpciones) btnOpciones.innerHTML = "...";
    }
});

/* ============================================================
   2. SENSOR PARA ABRIR EL MODAL Y CARGAR DATOS (LECTURA)
   ============================================================ */
document.addEventListener("click", function (e) {
    const overlayGeneral = document.getElementById("overlayEdicion");

    // Solo actúa si el modo edición (foco) está activo
    if (overlayGeneral && overlayGeneral.style.display === "block") {
        const boton = e.target.closest(".chip-button");

        if (boton) {
            e.preventDefault();
            e.stopImmediatePropagation();

            const idTipo = boton.getAttribute("data-id-tipo");

            fetch(`/Recursos/GetDatosTipoRecurso?id=${idTipo}`)
                .then(res => {
                    if (!res.ok) throw new Error("Error en la respuesta del servidor");
                    return res.json();
                })
                .then(data => {
                    const modalPadre = document.getElementById("modalEditarTipoRecurso");

                    if (modalPadre) {
                        // Llenado de campos con validación de existencia
                        const inputId = document.getElementById("editIdTipo");
                        const inputNombre = document.getElementById("editNombreInput");
                        const selectCat = document.getElementById("editCategoriaSelect");

                        if (inputId) inputId.value = data.id || data.Id;
                        if (inputNombre) inputNombre.value = data.nombre || data.Nombre;
                        if (selectCat) {
                            // Soporta tanto IdCategoria como idCategoria según el JSON
                            selectCat.value = data.idCategoria || data.IdCategoria;
                        }

                        // Mostrar modal
                        modalPadre.style.setProperty("display", "flex", "important");
                        console.log("✅ Datos cargados correctamente para ID:", idTipo);
                    }
                })
                .catch(err => console.error("❌ Error al cargar datos:", err));
        }
    }
}, true);

/* ============================================================
   3. EVENTO PARA GUARDAR CAMBIOS (ESCRITURA)
   ============================================================ */
const formEditar = document.getElementById("formEditarTipoRecurso");
if (formEditar) {
    formEditar.addEventListener("submit", function (e) {
        e.preventDefault();

        const idVal = document.getElementById("editIdTipo").value;
        const nombreVal = document.getElementById("editNombreInput").value;
        const categoriaVal = document.getElementById("editCategoriaSelect").value;

        const datos = {
            id: parseInt(idVal),
            nombre: nombreNombreVal,
            idCategoria: parseInt(categoriaVal)
        };

        fetch('/Recursos/EditarTipoRecurso', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(datos)
        })
            .then(response => {
                if (response.ok) {
                    alert("✅ Cambios guardados correctamente");
                    cerrarModalEdicion();
                    location.reload();
                } else {
                    alert("❌ Hubo un error al guardar");
                }
            })
            .catch(error => console.error("❌ Error al guardar:", error));
    });
}

/* ============================================================
   4. FUNCIONES DE APOYO
   ============================================================ */
function cerrarModalEdicion() {
    const modal = document.getElementById("modalEditarTipoRecurso");
    if (modal) {
        modal.style.display = "none";
    }
}