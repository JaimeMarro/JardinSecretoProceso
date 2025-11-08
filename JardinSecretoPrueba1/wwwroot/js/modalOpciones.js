document.addEventListener("DOMContentLoaded", function () {
    // --- SETUP ---
    // CORRECCIÓN CLAVE: Seleccionar el contenedor principal de pestañas.
    // Este elemento (categoryTabContent) siempre está visible y envuelve a TODAS las cuadrículas de productos.
    const productContainer = document.getElementById('categoryTabContent'); // Cambiado de '.row' a '#categoryTabContent'
    const modalElement = document.getElementById('modalOpcionesProducto');

    if (!productContainer || !modalElement) {
        console.error("Error crítico: El contenedor de pestañas o el modal no se encontraron.");
        return;
    }

    const modal = new bootstrap.Modal(modalElement);
    const modalNombreProducto = document.getElementById('modalNombreProducto');
    const opcionesSaborDiv = document.getElementById('opcionesSabor');
    const opcionesExtrasDiv = document.getElementById('opcionesExtras');
    const btnAgregarAlCarrito = document.getElementById('btnAgregarAlCarrito');
    let productoIdSeleccionado = 0;

    // --- EVENT LISTENER (DELEGACIÓN) ---
    // Ahora escucha los clics en cualquier parte dentro de #categoryTabContent
    productContainer.addEventListener('click', function (event) {
        // Usa closest() para buscar el botón btn-agregar-modal más cercano al elemento clickeado.
        const button = event.target.closest('.btn-agregar-modal');
        if (!button) return;

        event.preventDefault();
        productoIdSeleccionado = button.dataset.id;

        modalNombreProducto.textContent = 'Cargando...';
        opcionesSaborDiv.innerHTML = '<p class="text-white-50 small">Cargando sabores...</p>';
        opcionesExtrasDiv.innerHTML = '<p class="text-white-50 small">Cargando extras...</p>';
        modal.show();
        cargarOpciones(productoIdSeleccionado);
    });

    // --- FUNCIÓN PARA CARGAR OPCIONES (se mantiene igual) ---
    function cargarOpciones(productoId) {
        fetch(`/ProductosClientes/GetOpcionesProducto?productoId=${productoId}`)
            .then(response => response.json())
            .then(data => {
                modalNombreProducto.textContent = data.nombreProducto;
                opcionesSaborDiv.innerHTML = '';
                if (data.sabores.length > 0) {
                    data.sabores.forEach((sabor, index) => {
                        opcionesSaborDiv.innerHTML += `
                            <div class="form-check">
                                <input class="form-check-input" type="radio" name="sabor" id="sabor-${sabor.id}" value="${sabor.id}" ${index === 0 ? 'checked' : ''}>
                                <label class="form-check-label" for="sabor-${sabor.id}">
                                    ${sabor.nombre} ${sabor.precio > 0 ? `(+ $${sabor.precio.toFixed(2)})` : ''}
                                </label>
                            </div>`;
                    });
                } else {
                    opcionesSaborDiv.innerHTML = '<p class="text-white-50 small">No hay sabores para este producto.</p>';
                }

                opcionesExtrasDiv.innerHTML = '';
                if (data.extras.length > 0) {
                    data.extras.forEach(extra => {
                        opcionesExtrasDiv.innerHTML += `
                            <div class="form-check">
                                <input class="form-check-input extra-checkbox" type="checkbox" name="extras" id="extra-${extra.id}" value="${extra.id}">
                                <label class="form-check-label" for="extra-${extra.id}">${extra.nombre} (+ $${extra.precio.toFixed(2)})</label>
                            </div>`;
                    });
                    document.querySelectorAll('.extra-checkbox').forEach(c => c.onchange = () => {
                        if (document.querySelectorAll('.extra-checkbox:checked').length > 2) c.checked = false;
                    });
                } else {
                    opcionesExtrasDiv.innerHTML = '<p class="text-white-50 small">No hay extras para este producto.</p>';
                }
            });
    }

    // --- FUNCIÓN PARA AGREGAR AL CARRITO (se mantiene igual) ---
    btnAgregarAlCarrito.addEventListener('click', function () {
        const saborSeleccionado = document.querySelector('input[name="sabor"]:checked');
        const extrasSeleccionados = Array.from(document.querySelectorAll('input[name="extras"]:checked')).map(cb => cb.value);

        const tieneSabores = opcionesSaborDiv.querySelector('input[name="sabor"]');
        if (tieneSabores && !saborSeleccionado) {
            alert('Por favor, selecciona un sabor.');
            return;
        }

        const itemParaEnviar = {
            ProductoId: parseInt(productoIdSeleccionado),
            SaborId: tieneSabores ? parseInt(saborSeleccionado.value) : 0,
            ExtrasIds: extrasSeleccionados.map(id => parseInt(id)),
            Cantidad: 1
        };

        fetch('/Carrito/AgregarConOpciones', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(itemParaEnviar)
        }).then(response => {
            if (response.ok) {
                location.reload();
            } else {
                alert('Hubo un error al agregar el producto al carrito.');
            }
        });
    });
});