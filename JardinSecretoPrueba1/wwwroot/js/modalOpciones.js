document.addEventListener("DOMContentLoaded", () => {
    const btnsAgregar = document.querySelectorAll(".btn-agregar-modal");

    const modal = new bootstrap.Modal(document.getElementById("modalOpcionesProducto"));
    const modalNombre = document.getElementById("modalNombreProducto");
    const contenedorSabor = document.getElementById("opcionesSabor");
    const contenedorExtras = document.getElementById("opcionesExtras");
    const btnAgregarModal = document.getElementById("btnAgregarAlCarrito");

    let productoActual = null;
    let sabores = [];
    let extras = [];

    // 1. Abrir modal y traer opciones
    btnsAgregar.forEach(btn => {
        btn.addEventListener("click", async (e) => {
            e.preventDefault();

            //Con esta linea el script obtiene el id del producto al que se le dio click
            const productoId = btn.getAttribute("data-id"); //Le decimos que busque el apartado de data-id
            productoActual = { id: productoId, nombre: btn.closest(".card").querySelector(".card-title").textContent };

            modalNombre.textContent = productoActual.nombre;
            contenedorSabor.innerHTML = "<p class='text-muted small'>Cargando sabores...</p>";
            // Usamos el nuevo nombre y un texto <p> que tiene más sentido para un <div>
            contenedorExtras.innerHTML = "<p class='text-muted small'>Cargando extras...</p>";

            // Traemos sabores y extras desde el controller
            try {
                const res = await fetch(`/ProductosClientes/GetOpciones?id=${productoId}`);
                const data = await res.json();
                sabores = data.sabores;
                extras = data.extras;

                // Renderizamos sabores (radio)
                contenedorSabor.innerHTML = "";
                sabores.forEach((sabor, i) => {
                    const radio = document.createElement("div");
                    radio.classList.add("form-check");
                    radio.innerHTML = `
                        <input class="form-check-input" type="radio" name="saborProducto" id="sabor${i}" value="${sabor.id}" data-precio="${sabor.precio ?? 0}">
                        <label class="form-check-label" for="sabor${i}">${sabor.nombre} ${sabor.precio > 0 ? `(+ $${sabor.precio.toFixed(2)})` : ""}</label>
                    `;
                    contenedorSabor.appendChild(radio);
                });

                // Renderizamos extras (checkboxes)
                contenedorExtras.innerHTML = ""; // Limpiamos el "Cargando..."
                if (extras.length === 0) {
                    contenedorExtras.innerHTML = "<p class='text-muted small'>No hay extras para este producto.</p>";
                }
                extras.forEach((extra, i) => {
                    const check = document.createElement("div");
                    check.classList.add("form-check");
                    // Creamos un input 'checkbox'
                    // Guardamos los datos en atributos 'data-' para leerlos fácil
                    check.innerHTML = `
                    <input class="form-check-input extra-checkbox" type="checkbox" 
                           value="${extra.id}" id="extra${i}" 
                           data-nombre="${extra.nombre}" 
                           data-precio="${extra.precio ?? 0}">
                    <label class="form-check-label" for="extra${i}">
                        ${extra.nombre} ${extra.precio > 0 ? `(+ $${extra.precio.toFixed(2)})` : ""}
                    </label>
    `;
                    contenedorExtras.appendChild(check);
                });

            } catch (error) {
                contenedorSabor.innerHTML = "<p class='text-danger'>Error cargando sabores</p>";
                selectExtras.innerHTML = "<option disabled>Error</option>";
                console.error(error);
            }

            modal.show();
        });
    });

    // 2. Control máximo 2 extras (ahora en el contenedor)
    contenedorExtras.addEventListener("change", (e) => {
        // Nos aseguramos de que el clic fue en un checkbox de clase 'extra-checkbox'
        if (e.target.classList.contains("extra-checkbox")) {
            // Contamos cuántos checkboxes están marcados
            const checkboxesMarcados = contenedorExtras.querySelectorAll(".extra-checkbox:checked");
            if (checkboxesMarcados.length > 2) {
                alert("Solo puedes seleccionar un máximo de 2 extras.");
                // Desmarcamos el que acaba de marcar
                e.target.checked = false;
            }
        }
    });

    // 3. Agregar al carrito desde modal
    btnAgregarModal.addEventListener("click", async () => { // <-- Añadimos "async" para poder usar "await"

        // --- 1. Validar Sabor ---
        const saborSeleccionado = document.querySelector("input[name='saborProducto']:checked");
        if (!saborSeleccionado) {
            alert("Debes seleccionar un sabor.");
            return;
        }

        // --- 2. Preparar Extras (con checkboxes) ---
        // Buscamos todos los checkboxes que estén marcados (:checked)
        const extrasCheckboxes = contenedorExtras.querySelectorAll(".extra-checkbox:checked");

        // Convertimos la lista de checkboxes en nuestra lista de objetos
        const extrasSeleccionados = Array.from(extrasCheckboxes).map(check => {
            return {
                Id: parseInt(check.value),
                Nombre: check.dataset.nombre, // Leemos desde 'data-nombre'
                Precio: parseFloat(check.dataset.precio) // Leemos desde 'data-precio'
            };
        });

        // --- 3. Construir el Objeto Completo ---
        const itemParaCarrito = {
            ProductoId: parseInt(productoActual.id),
            Cantidad: 1,
            SaborId: parseInt(saborSeleccionado.value),
            SaborNombre: saborSeleccionado.nextElementSibling.textContent,
            SaborPrecio: parseFloat(saborSeleccionado.dataset.precio),
            Extras: extrasSeleccionados
        };

        // --- 4. Enviar al Servidor (El nuevo código) ---
        try {
            const response = await fetch('/Carrito/AgregarConOpciones', {
                method: 'POST', // Le decimos que es un envío
                headers: {
                    'Content-Type': 'application/json' // Le decimos que es JSON
                },
                body: JSON.stringify(itemParaCarrito) // Convertimos nuestro objeto a string JSON
            });

            // 5. Revisar la respuesta del servidor
            if (response.ok) {
                // ¡Éxito! El servidor dijo "OK"
                alert("Producto agregado al carrito!");
                modal.hide();
                // La forma más simple de actualizar el contador del carrito es recargar la página
                window.location.reload();
            } else {
                // El servidor respondió con un error (ej: 404, 500)
                alert("Hubo un error al guardar el producto.");
            }
        } catch (error) {
            // Error de red (ej: se cayó el internet)
            console.error("Error de red:", error);
            alert("Error de conexión al agregar el producto.");
        }
    });
});
