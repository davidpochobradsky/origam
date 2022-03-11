const puppeteer = require("puppeteer");
const { backEndUrl} = require('./additionalConfig');
const { sleep, xPathContainsClass, openMenuItem, login, getRowCountData, catchRequests, waitForRowCount,
  waitForRowCountData, switchToFormPerspective, inputByPressingKeys, switchLanguageTo, waitForFocus,
  switchToTablePerspective
} = require('./testTools');
const {widgetsMenuItemId, sectionsMenuItemId, masterDerailMenuItemId, topMenuHeader,
  allDataTypesLazyMenuItemsId} = require("./modelIds");
const { putNumericTestDataToAllDataTypes, clearScreenConfiguration,
  restoreAllDataTypesTable} = require("./dbTools");
const {
  openFilters,
  setFilter,
  setTwoFieldFilter,
  setDateFilter,
  setTwoFieldDateFilter,
  setComboFilter
} = require("./filterTestTools");

let browser;
let page;

beforeAll(async() => {
  await clearScreenConfiguration();
});

beforeEach(async () => {
  browser = await puppeteer.launch({
    ignoreHTTPSErrors: true,
    //devtools: true,
    headless: false,
    defaultViewport: {
      width: 1024,
      height: 768,
    },
    args: ["--no-sandbox", "--disable-setuid-sandbox"],
  });
  page = await browser.newPage();
  await page.goto(backEndUrl);
  await page.evaluate(() => {
    localStorage.setItem("debugCloseAllForms", "1");
  });
});

afterEach(async () => {
  let pages = await browser.pages();
  await Promise.all(pages.map(page => page.close()));
  await sleep(200);
  if(browser) await browser.close();
  browser = undefined;
});

const dataViewId = "dataView_e67865b0-ce91-413c-bed7-1da486399633";
const currencyPropertyId = "ff303553-9c3e-407f-b63c-a981c9597aee";
const decimalSeparator = ",";
const thousandsSeparator = " ";

describe("Html client", () => {
  it("Should format float number after input", async () => {
    await restoreAllDataTypesTable();
    await switchLanguageTo({locale: "cs-CZ", page: page});
    await login(page);
    await openMenuItem(
      page,
      [
        topMenuHeader,
        widgetsMenuItemId,
        allDataTypesLazyMenuItemsId
      ]);

    await waitForRowCountData(page, dataViewId,2099);

    await sleep(300);

    await switchToFormPerspective({
      page: page,
      aPropertyId: currencyPropertyId
    });
    await sleep(300);
    const numberEditor = await page.waitForSelector(
      `#editor_${currencyPropertyId}`,
      {visible: true});
    await page.evaluate(x => x.focus(), numberEditor);
    await sleep(500);

    await inputByPressingKeys({page: page, value: `-123456${decimalSeparator}789`})

    await sleep(200);

    await waitForFocus({
      elementId: `editor_${currencyPropertyId}`,
      page: page
    })
    await sleep(200);
    await page.keyboard.press("Tab");

    await sleep(1000);

    const editorValue = await page.evaluate(x => x.value, numberEditor);
    expect(editorValue).toBe(`-123${thousandsSeparator}456${decimalSeparator}789`);

    await switchToTablePerspective({page: page});

    await sleep(500); // to make sure there is no error after switching to table perspective
  });
  it("Should reformat float number after changes", async () => {
    await putNumericTestDataToAllDataTypes();
    await switchLanguageTo({locale: "cs-CZ", page: page});
    await login(page);
    await openMenuItem(
      page,
      [
        topMenuHeader,
        widgetsMenuItemId,
        allDataTypesLazyMenuItemsId
      ]);

    await waitForRowCountData(page, dataViewId,1);

    await sleep(300);

    await switchToFormPerspective({
      page: page,
      aPropertyId: currencyPropertyId
    });
    await sleep(300);
    const numberEditor = await page.waitForSelector(
      `#editor_${currencyPropertyId}`,
      {visible: true});
    await page.evaluate(x => {
      x.focus();
      x.setSelectionRange(5, 5);
    }, numberEditor);
    await sleep(200);

    await page.keyboard.press("Backspace");

    await sleep(500);

    const cursorPosition = await page.evaluate(x => x.selectionStart, numberEditor);
    expect(cursorPosition).toBe(4);

    await page.keyboard.press("Tab");

    await sleep(1000);

    const editorValue = await page.evaluate(x => x.value, numberEditor);
    expect(editorValue).toBe(`12${thousandsSeparator}356${decimalSeparator}789`);
  });
});
