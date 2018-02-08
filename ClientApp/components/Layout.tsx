import * as React from 'react';
import { History } from './History'
import { FetchPrices } from './FetchPrices'

export interface LayoutProps {
    children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps, {}> {
    public render() {
        return <div className='container'>
            { this.props.children }
            {/* <History /> */}
        </div>;
    }
}
