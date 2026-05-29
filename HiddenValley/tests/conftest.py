import pytest
import requests

BASE_URL = "http://localhost:7084"

@pytest.fixture(scope="session")
def api():
    """Verifica que la API esté corriendo antes de ejecutar las pruebas"""
    try:
        r = requests.get(f"{BASE_URL}/api/cabanas", timeout=5)
        assert r.status_code == 200, "La API no respondió correctamente"
    except Exception as e:
        pytest.fail(f"La API no está corriendo en {BASE_URL}. ¿Levantaste Docker? Error: {e}")
    return BASE_URL