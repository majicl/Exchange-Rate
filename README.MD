# Exchange Rates API

Exchange rates API is a free service for current and historical foreign exchange rates [published by the European Central Bank](https://www.ecb.europa.eu/stats/policy_and_exchange_rates/euro_reference_exchange_rates/html/index.en.html).

## Usage

```http
POST /api/currency/exchangeRate
```

with the filter parameters:

```json
{
  "dates": Array[String],
  "baseCurrency": String,
  "targetCurrency": String
}
```

## Available Scripts

It is used to install all dependencies for a project.<br />
**It is necessary to run before staring the app**.

#### `dotnet restore`

In the API project directory, you can run:

#### `dotnet run`

Runs the app in the development mode.<br />
Open [http://localhost:5000](http://localhost:5000) to view it in the browser.

### How to run the test?

#### `dotnet test`

### How to publish?

#### `dotnet publish -c Release -r win-x64 -o ./output --self-contained`

... to be continued!
