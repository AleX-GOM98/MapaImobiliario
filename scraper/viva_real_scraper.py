import asyncio
from playwright.async_api import async_playwright
import re
import json

URL = "https://www.vivareal.com.br/venda/minas-gerais/belo-horizonte/"

def extract_price(price_str):
    match = re.search(r'R\$\s?([\d\.,]+)', price_str)
    return match.group(1).replace('.', '').replace(',', '.') if match else None

def extract_condominio_iptu(price_str):
    condominio_match = re.search(r'Cond\.\sR\$\s?([\d\.,]+)', price_str)
    iptu_match = re.search(r'IPTU\sR\$\s?([\d\.,]+)', price_str)
    condominio = condominio_match.group(1).replace('.', '').replace(',', '.') if condominio_match else None
    iptu = iptu_match.group(1).replace('.', '').replace(',', '.') if iptu_match else None
    return condominio, iptu

async def buscar_endereco_completo(context, url):
    try:
        page = await context.new_page()
        await page.goto(url, timeout=60000)
        await page.wait_for_timeout(2000)
        endereco_element = await page.query_selector('span[data-cy="listing-address"]')
        endereco_completo = await endereco_element.inner_text() if endereco_element else ""
        await page.close()
        return endereco_completo.strip()
    except Exception as e:
        print(f"Erro ao acessar detalhe: {e}")
        return ""

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
                preco_el = await card.query_selector("div[data-cy='rp-cardProperty-price-txt']")
                link_el = await card.query_selector("a")

                titulo = await titulo_el.inner_text() if titulo_el else ""
                preco = await preco_el.inner_text() if preco_el else ""
                condominio, iptu = extract_condominio_iptu(preco)
                preco_formatado = extract_price(preco)

                link = await link_el.get_attribute("href") if link_el else ""
                if link and not link.startswith("https://www.vivareal.com.br"):
                    link = f"https://www.vivareal.com.br{link.strip()}"

                endereco_el = await page.query_selector('span[data-cy="listing-address"]')
                if not endereco_el:
                    # Tentar alternativa (pode variar)
                    endereco_el = await page.query_selector('p[data-cy="listing-address"]')

                endereco = await buscar_endereco_completo(context, link) if link else ""

                imoveis.append({
                    "titulo": titulo.strip(),
                    "endereco": endereco,
                    "preco": preco_formatado,
                    "condominio": condominio,
                    "iptu": iptu,
                    "link": link
                })
            except Exception as e:
                print("Erro ao extrair item:", e)

        await browser.close()
        return imoveis

if __name__ == "__main__":
    dados = asyncio.run(buscar_imoveis_viva_real())    
    print(json.dumps(dados, ensure_ascii=False, indent=2))
