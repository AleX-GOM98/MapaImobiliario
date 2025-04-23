import asyncio
from playwright.async_api import async_playwright
import re

URL = "https://www.vivareal.com.br/venda/minas-gerais/belo-horizonte/"

def extract_price(price_str):
    # Expressão regular para capturar o valor do preço
    price_match = re.search(r'R\$\s?([\d\.,]+)', price_str)
    if price_match:
        # Remove os pontos e substitui a vírgula por ponto para formato numérico
        return price_match.group(1).replace('.', '').replace(',', '.')
    return None

def extract_condominio_iptu(price_str):
    # Expressão regular para capturar valores de condomínio e IPTU
    condominio_match = re.search(r'Cond\.\sR\$\s?([\d\.,]+)', price_str)
    iptu_match = re.search(r'IPTU\sR\$\s?([\d\.,]+)', price_str)
    
    condominio = condominio_match.group(1).replace('.', '').replace(',', '.') if condominio_match else None
    iptu = iptu_match.group(1).replace('.', '').replace(',', '.') if iptu_match else None
    
    return condominio, iptu

async def buscar_imoveis_viva_real():
    async with async_playwright() as p:
        browser = await p.chromium.launch(headless=True)
        context = await browser.new_context(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36"
        )
        page = await context.new_page()
        await page.set_viewport_size({"width": 1280, "height": 800})
        await page.goto(URL, timeout=60000)

        await page.wait_for_timeout(3000)
        await page.mouse.wheel(0, 3000)
        await page.wait_for_timeout(2000)

        await page.wait_for_selector('[data-cy="rp-property-cd"]', timeout=60000)

        cards = await page.query_selector_all('[data-cy="rp-property-cd"]')

        imoveis = []
        for card in cards:
            try:
                titulo_el = await card.query_selector("h2")
                endereco_el = await card.query_selector("p")
                preco_el = await card.query_selector("div[data-cy='rp-cardProperty-price-txt']")
                condominio_el = await card.query_selector("p.text-neutra1-119.overflow-hidden.text-ellipsis")  # Verificar se existe esse campo
                link_el = await card.query_selector("a")

                titulo = await titulo_el.inner_text() if titulo_el else ""
                endereco = await endereco_el.inner_text() if endereco_el else ""
                preco = await preco_el.inner_text() if preco_el else ""
                
                # Tentar pegar o condomínio e IPTU
                condominio, iptu = extract_condominio_iptu(preco)
                preco_formatado = extract_price(preco)

                link = await link_el.get_attribute("href") if link_el else ""

                # Verificar se o link já contém o domínio e, se necessário, concatenar
                if link and not link.startswith("https://www.vivareal.com.br"):
                    link = f"https://www.vivareal.com.br{link.strip()}"

                imoveis.append({
                    "titulo": titulo.strip(),
                    "endereco": endereco.strip(),
                    "preco": preco_formatado,  # Usando o preço formatado
                    "condominio": condominio,  # Condominio extraído
                    "iptu": iptu,  # IPTU extraído
                    "link": link
                })
            except Exception as e:
                print("Erro ao extrair item:", e)

        await browser.close()
        return imoveis

if __name__ == "__main__":
    dados = asyncio.run(buscar_imoveis_viva_real())
    for imovel in dados:
        print(imovel)
