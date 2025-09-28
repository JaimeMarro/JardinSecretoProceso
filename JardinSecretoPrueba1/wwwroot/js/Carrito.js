document.addEventListener("DOMContentLoaded", () => {
    const STORAGE_KEY = "carrito_v1";
    const tbody = document.querySelector("#tabla-carrito tbody");
    const totalEl = document.getElementById("total");
    const btnVaciar = document.getElementById("vaciar");
    const btnEnviar = document.getElementById("enviar");

    // Leer carrito del localStorage
    function leerCarrito() {
        try {
            const raw = localStorage.getItem(STORAGE_KEY);
            return raw ? JSON.parse(raw) : [];
        } catch (e) {
            console.error("Error al leer carrito:", e);
            return [];
        }
    }

    // Guardar carrito y refrescar tabla
    function guardarCarrito(arr) {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(arr));
        renderizar();
    }

    // Agregar producto al carrito
    function agregarAlCarrito(id, nombre, precio) {
        let carrito = leerCarrito();
        let existente = carrito.find(p => p.id == id);

        if (existente) {
            existente.cantidad += 1;
        } else {
            carrito.push({
                id: id,
                nombre: nombre,
                precio: precio,
                cantidad: 1
            });
        }

        guardarCarrito(carrito);
        alert(`${nombre} agregado al carrito`);
    }

    // Renderizar carrito en la tabla
    function renderizar() {
        if (!tbody) return; // <- si estoy en ProductosClientes, no existe la tabla
        const carrito = leerCarrito();
        tbody.innerHTML = "";
        let total = 0;

        if (carrito.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center">Tu carrito está vacío.</td></tr>`;
            totalEl.textContent = "0.00";
            return;
        }

        carrito.forEach((item, i) => {
            const precio = Number(item.precio) || 0;
            const cantidad = Number(item.cantidad) || 0;
            const subtotal = precio * cantidad;
            total += subtotal;

            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${escapeHtml(item.nombre)}</td>
                <td>$${precio.toFixed(2)}</td>
                <td>
                    <button class="menos btn btn-sm btn-secondary" data-index="${i}">-</button>
                    ${cantidad}
                    <button class="mas btn btn-sm btn-secondary" data-index="${i}">+</button>
                </td>
                <td>$${subtotal.toFixed(2)}</td>
                <td><button class="eliminar btn btn-sm btn-danger" data-index="${i}">Eliminar</button></td>
            `;
            tbody.appendChild(tr);
        });

        totalEl.textContent = total.toFixed(2);
    }

    function escapeHtml(text) {
        if (!text) return "";
        return text.replace(/[&<>"'`=\/]/g, s => "&#" + s.charCodeAt(0) + ";");
    }

    // Escuchar clicks en botones "Agregar al carrito" (vista ProductosClientes/Index)
    document.body.addEventListener("click", (e) => {
        if (e.target.classList.contains("add-to-cart")) {
            const id = e.target.dataset.id;
            const nombre = e.target.dataset.nombre;
            const precio = parseFloat(e.target.dataset.precio);
            agregarAlCarrito(id, nombre, precio);
        }
    });

    // Acciones dentro de la tabla (solo en vista Carrito/Index)
    if (tbody) {
        tbody.addEventListener("click", (e) => {
            let carrito = leerCarrito();
            if (e.target.matches(".mas")) {
                const i = Number(e.target.dataset.index);
                carrito[i].cantidad += 1;
                guardarCarrito(carrito);
            }
            if (e.target.matches(".menos")) {
                const i = Number(e.target.dataset.index);
                carrito[i].cantidad -= 1;
                if (carrito[i].cantidad <= 0) carrito.splice(i, 1);
                guardarCarrito(carrito);
            }
            if (e.target.matches(".eliminar")) {
                const i = Number(e.target.dataset.index);
                carrito.splice(i, 1);
                guardarCarrito(carrito);
            }
        });

        // Vaciar carrito
        btnVaciar.addEventListener("click", () => {
            if (!confirm("¿Vaciar todo el carrito?")) return;
            localStorage.removeItem(STORAGE_KEY);
            renderizar();
        });

        // Enviar por WhatsApp
        btnEnviar.addEventListener("click", () => {
            const carrito = leerCarrito();
            if (carrito.length === 0) {
                alert("El carrito está vacío");
                return;
            }

            // Leer datos del cliente desde spans ocultos
            const nombreCliente = document.getElementById("cliente-nombre")?.dataset.nombre || "Invitado";
            const tipoPedido = document.getElementById("cliente-pedido")?.dataset.tipo || "No especificado";


            let mensaje = `Hola, soy ${nombreCliente}.%0A Tipo de pedido: ${tipoPedido}.%0A%0A Quiero este pedido:%0A`;
            carrito.forEach(it => {
                mensaje += `- ${it.nombre} x${it.cantidad}: $${(it.precio * it.cantidad).toFixed(2)}%0A`;
            });
            const total = carrito.reduce((s, it) => s + it.precio * it.cantidad, 0);
            mensaje += `Total: $${total.toFixed(2)}`;

            const telefono = "70857606" // tu número de WhatsApp
            window.open(`https://wa.me/${telefono}?text=${mensaje}`, "_blank");
        });

        // Render inicial
        renderizar();
    }
});
