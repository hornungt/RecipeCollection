//'use strict';
const puppeteer = require('puppeteer');
const fs = require('fs');

let url = process.argv[2];
let destination = process.argv[3];

if (!destination.endsWith(".pdf")) {
    destination = destination + ".pdf";
}

(async () => {
    const browser = await puppeteer.launch();
    const page = await browser.newPage();
    await page.goto(url);
    await fs.mkdir(`./pdfs`, { recursive: true }, (err) => {
        if (err) throw err;
    });
    await page.pdf({ path: destination, format: 'A4' });

    await browser.close();
})();
