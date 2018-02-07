import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { FetchPrices } from './FetchPrices'

export class Home extends React.Component<RouteComponentProps<{}>, {}> {
    private currency = 'Bitcoin'
    private sellPrice = 8353.70
    private buyPrice = 8156.24

    /* TODO: ideas to add to this site:
        1) Today 13 Jan 2017
        Add last prices update time: 12:00:12 and next update in 29 sec (countdown)
        2) Add history data, so 20 last updates are shown below*/

    public render() {
        return <div>
            <div className='text-center w-100 p-4'>
                <h1>Welcome to CoinTree Viewer</h1>
            </div>
            <p>Check out our current prices:</p>
            <div className="container">
                <FetchPrices />
            </div>
        </div>;
    }
}
