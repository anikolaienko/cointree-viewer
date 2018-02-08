import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { HubConnection } from '@aspnet/signalr-client';

interface FetchPricesState {
    price: CurrencyPrice;
    loading: boolean;
    desiredBuyPrice: number;
    desiredSellPrice: number;
}

export class FetchPrices extends React.Component<{}, FetchPricesState> {

    constructor() {
        super();
        this.handleBuyChange = this.handleBuyChange.bind(this);
        this.handleSellChange = this.handleSellChange.bind(this);
        this.state = { price: {name: "", buyPrice: 0, buyPriceDiff: 0, sellPrice: 0, sellPriceDiff: 0, timestamp: new Date()},
            loading: true,
            desiredBuyPrice: 0,
            desiredSellPrice: 0 };

        let connection = new HubConnection('/prices');

        connection.on('updateCurrencyPrice', data => {
            this.setState({loading: false, price: data })
            console.log(data);
        });
        
        connection.start()
            .then(() => connection.invoke('GetLatestPrice')
                .then(price => {
                    this.setState({loading: false, price: price });
        }));
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderPricesTable(this.state.price);

        return <div>
                <h3>Currency Price</h3>
                { contents }
            </div>;
        
    }

    private handleBuyChange(event: any) {
        this.setState({desiredBuyPrice: event.target.value});
    }

    private handleSellChange(event: any) {
        this.setState({desiredSellPrice: event.target.value});
    }

    private renderPricesTable(price: CurrencyPrice) {
        return <table className='table'>
            <thead>
                <tr>
                    <th>Currency</th>
                    <th>Buy Price</th>
                    <th>Buy Price Diff</th>
                    <th>Sell Price</th>
                    <th>Sell Price Diff</th>
                </tr>
            </thead>
            <tbody>
                <tr key={ price.name }>
                    <td>{ price.name }</td>
                    <td className="price">{ price.buyPrice }</td>
                    <td><p className={price.buyPriceDiff < 0 ? 'price-down' : (price.buyPriceDiff > 0 ? 'price-up' : '')}>{ price.buyPriceDiff.toFixed(2) }</p></td>
                    <td className="price">{ price.sellPrice }</td>
                    <td><p className={price.sellPriceDiff < 0 ? 'price-down' : (price.sellPriceDiff > 0 ? 'price-up' : '')}>{ price.sellPriceDiff.toFixed(2) }</p></td>
                </tr>
                <tr>
                    <td>Desired Buy Price</td>
                    <td className="price"><input className={this.state.desiredBuyPrice != 0
                        && this.state.desiredBuyPrice >= this.state.price.buyPrice
                            ? "desired"
                            : ""}
                        type="number" step="0.01" value={this.state.desiredBuyPrice} onChange={this.handleBuyChange}/></td>
                    <td>Desired Sell Price</td>
                    <td className="price"><input className={this.state.desiredSellPrice <= this.state.price.sellPrice
                            ? "desired"
                            : ""}
                        type="number" step="0.01" value={this.state.desiredSellPrice}/></td>
                    <td></td>
                </tr>
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
    timestamp: Date;
}
