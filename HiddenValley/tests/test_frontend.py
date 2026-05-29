import pytest
from playwright.sync_api import Page, expect

FRONTEND_URL = "http://localhost:7075"


def ir_a(page: Page, href: str):
    """Hace clic en el link del menú de navegación usando el href exacto"""
    page.goto(FRONTEND_URL)
    page.wait_for_timeout(2000)
    page.locator(f"nav a[href='{href}']").click()
    page.wait_for_timeout(2000)


class TestDashboard:

    def test_dashboard_carga(self, page: Page):
        ir_a(page, "dashboard")
        expect(page.locator("text=Operaciones del Dia")).to_be_visible(timeout=10000)

    def test_dashboard_tiene_tabla(self, page: Page):
        ir_a(page, "dashboard")
        expect(page.locator("text=Resumen en tiempo real")).to_be_visible(timeout=10000)


class TestCabanas:

    def test_cabanas_carga(self, page: Page):
        ir_a(page, "cabanas")
        expect(page.locator("text=Gestión de Cabañas")).to_be_visible(timeout=10000)

    def test_cabanas_boton_registrar_visible(self, page: Page):
        ir_a(page, "cabanas")
        expect(page.locator("text=Registrar Nueva Cabaña")).to_be_visible(timeout=10000)

    def test_cabanas_abrir_modal_registro(self, page: Page):
        ir_a(page, "cabanas")
        page.locator("text=Registrar Nueva Cabaña").click()
        expect(page.locator("text=Nueva Cabaña en Sistema")).to_be_visible(timeout=5000)

    def test_cabanas_cerrar_modal(self, page: Page):
        ir_a(page, "cabanas")
        page.locator("text=Registrar Nueva Cabaña").click()
        page.locator("text=Nueva Cabaña en Sistema").wait_for(timeout=5000)
        page.locator("text=Cancelar").click()
        expect(page.locator("text=Nueva Cabaña en Sistema")).not_to_be_visible()


class TestReservaciones:

    def test_reservaciones_carga(self, page: Page):
        ir_a(page, "reservaciones")
        expect(page.locator("text=Panel de Reservaciones")).to_be_visible(timeout=10000)

    def test_reservaciones_boton_nueva_visible(self, page: Page):
        ir_a(page, "reservaciones")
        expect(page.locator("text=Nueva Reservación")).to_be_visible(timeout=10000)

    def test_reservaciones_abrir_modal_crear(self, page: Page):
        ir_a(page, "reservaciones")
        page.locator("text=Nueva Reservación").click()
        expect(page.locator("text=Nueva Reservación").nth(1)).to_be_visible(timeout=5000)

    def test_reservaciones_buscador(self, page: Page):
        ir_a(page, "reservaciones")
        page.locator("input[placeholder*='Buscar']").fill("Juan")
        page.locator("text=Buscar").click()
        page.wait_for_timeout(1000)
        expect(page.locator("text=Panel de Reservaciones")).to_be_visible()