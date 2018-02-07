import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { HubConnection } from '@aspnet/signalr-client'

interface FetchPricesState {
    prices: CurrencyPrice[];
    loading: boolean;
}

export class FetchPrices extends React.Component<{}, FetchPricesState> {
    constructor() {
        super();
        this.state = { prices: [], loading: true };

        let connection = new HubConnection('/prices');

        connection.on('updateCurrencyPrice', data => {
            this.setState({loading: false, prices: [data]})
            console.log(data);
        });
        
        connection.start()
            .then(() => connection.invoke('GetLatestPrice')
                .then(price => {
                    this.setState({loading: false, prices: [price]});
        }));

        // fetch('api/GetPrices')
        //     .then(response => response.json() as Promise<CurrencyPrice[]>)
        //     .then(data => {
        //         this.setState({ prices: data, loading: false });
        //     });
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : FetchPrices.renderPricesTable(this.state.prices);

        return <div>
                <h3>Currency Price</h3>
                { contents }
            </div>;
        
        // <div className="row">
        //         <div className="col">
        //             {currency}
        //         </div>
        //         <div className="col">
        //             {sellPrice}
        //         </div>
        //         <div className="col">
        //             {buyPrice}
        //         </div>
        //     </div>;
        
    }

    private static renderPricesTable(prices: CurrencyPrice[]) {
        return <table className='table'>
            <thead>
                <tr>
                    <th>Currency</th>
                    <th>Buy Price</th>
                    <th>Sell Price</th>
                </tr>
            </thead>
            <tbody>
            {prices.map(price =>
                <tr key={ price.name }>
                    <td>{ price.name }</td>
                    <td>$ { price.buyPrice }</td>
                    <td>$ { price.sellPrice }</td>
                </tr>
            )}
            </tbody>
        </table>;
    }
}

interface CurrencyPrice {
    name: string;
    buyPrice: number;
    buyPriceDiff: number;
    sellPrice: number;
    sellPriceDiff: number;
}
