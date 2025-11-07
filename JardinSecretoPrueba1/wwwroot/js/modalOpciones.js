document.addEventListener("DOMContentLoaded", function () {
    // --- SETUP ---
    const productContainer = document.querySelector('.row');
    const modalElement = document.getElementById('modalOpcionesProducto');

    if (!productContainer || !modalElement) {
        console.error("Error crítico: El contenedor de productos o el modal no se encontraron.");
        return;
    }

    const modal = new bootstrap.Modal(modalElement);
    const modalNombreProducto = document.getElementById('modalNombreProducto');
    const opcionesSaborDiv = document.getElementById('opcionesSabor');
    const opcionesExtrasDiv = document.getElementById('opcionesExtras');
    const btnAgregarAlCarrito = document.getElementById('btnAgregarAlCarrito');
    let productoIdSeleccionado = 0;

    // --- EVENT LISTENER ---
    productContainer.addEventListener('click', function (event) {
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

    // --- FUNCIÓN PARA CARGAR OPCIONES ---
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

    // --- FUNCIÓN PARA AGREGAR AL CARRITO ---
    btnAgregarAlCarrito.addEventListener('click', function () {
        const saborSeleccionado = document.querySelector('input[name="sabor"]:checked');
        const extrasSeleccionados = Array.from(document.querySelectorAll('input[name="extras"]:checked')).map(cb => cb.value);

        const tieneSabores = opcionesSaborDiv.querySelector('input[name="sabor"]');
        if (tieneSabores && !saborSeleccionado) {
            alert('Por favor, selecciona un sabor.');
            return;
        }

        // El objeto que se envía debe coincidir con el modelo CarritoItem que espera el controlador
        const itemParaEnviar = {
            ProductoId: parseInt(productoIdSeleccionado),
            SaborId: tieneSabores ? parseInt(saborSeleccionado.value) : 0,
            ExtrasIds: extrasSeleccionados.map(id => parseInt(id)),
            Cantidad: 1 // Siempre se agrega 1 desde el modal
        };

        // ===== INICIO DE LA CORRECCIÓN =====
        fetch('/Carrito/AgregarConOpciones', { // <-- SE CORRIGIÓ LA URL
            // ===== FIN DE LA CORRECCIÓN =====
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(itemParaEnviar) // Se envía el objeto completo
        }).then(response => {
            if (response.ok) {
                location.reload(); // Recarga la página para ver el carrito actualizado
            } else {
                alert('Hubo un error al agregar el producto al carrito.');
            }
        });
    });
});