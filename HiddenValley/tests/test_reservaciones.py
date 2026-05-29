import requests
import pytest
import time
from datetime import date, timedelta

BASE_URL = "http://localhost:7084"

class TestPersonasYClientes:

    def test_crear_persona(self):
        ts = int(time.time())
        payload = {
            "nombres": "Test",
            "apellidos": "E2E",
            "dpi": str(ts)[:13].ljust(13, '0'),
            "telefono": "88881234",
            "gmail": f"test{ts}@prueba.com",
            "direccion": "Jalapa, Guatemala"
        }
        r = requests.post(f"{BASE_URL}/api/personas", json=payload)
        assert r.status_code in [200, 201], f"Error al crear persona: {r.text}"

    def test_buscar_cliente_existente(self):
        r = requests.get(f"{BASE_URL}/api/clientes/buscar", params={"filtro": "55512345"})
        assert r.status_code == 200, f"Error al buscar cliente: {r.text}"
        data = r.json()
        assert data is not None

    def test_listar_cabanas(self):
        r = requests.get(f"{BASE_URL}/api/cabanas")
        assert r.status_code == 200


class TestReservaciones:

    def test_crear_reservacion_exitosa(self):
        hoy = date.today()
        entrada = (hoy + timedelta(days=5)).isoformat()
        salida  = (hoy + timedelta(days=8)).isoformat()

        # Primero cancelar cualquier reserva activa del cliente 1
        lista = requests.get(f"{BASE_URL}/api/reservaciones").json()
        items = lista if isinstance(lista, list) else lista.get("items", [])
        for reserva in items:
            if reserva.get("idCliente") == 1 and reserva.get("estadoReserva") not in ["Cancelada"]:
                requests.patch(f"{BASE_URL}/api/reservaciones/{reserva['id']}", json={"estadoReserva": "Cancelada"})

        payload = {
            "idCliente": 1,
            "telefono": "55512345",
            "idCabana": 1,
            "fechaEntrada": entrada,
            "fechaSalida": salida,
            "cantidadPersonas": 2,
            "idEmpleado": 1
        }
        r = requests.post(f"{BASE_URL}/api/reservaciones", json=payload)
        assert r.status_code == 201, f"Error al crear reservacion: {r.text}"

    def test_reservacion_fechas_invalidas(self):
        hoy = date.today()
        payload = {
            "idCliente": 1,
            "telefono": "55512345",
            "idCabana": 1,
            "fechaEntrada": (hoy + timedelta(days=10)).isoformat(),
            "fechaSalida":  (hoy + timedelta(days=5)).isoformat(),
            "cantidadPersonas": 2
        }
        r = requests.post(f"{BASE_URL}/api/reservaciones", json=payload)
        assert r.status_code == 400

    def test_listar_reservaciones(self):
        r = requests.get(f"{BASE_URL}/api/reservaciones")
        assert r.status_code == 200

    def test_dashboard(self):
        r = requests.get(f"{BASE_URL}/api/dashboard")
        assert r.status_code == 200
        data = r.json()
        assert "cabanasDisponibles" in data
        assert "cabanasOcupadasHoy" in data