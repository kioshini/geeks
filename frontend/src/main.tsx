import React from 'react'
import ReactDOM from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import './index.css'
import { CatalogPage } from './pages/Catalog'
import { CartPage } from './pages/Cart'
import { CheckoutPage } from './pages/Checkout'
import { AppLayout } from './shared/AppLayout'
import { UnitProvider } from './contexts/UnitContext'

const router = createBrowserRouter([
	{
		path: '/',
		element: <AppLayout />,
		children: [
			{ index: true, element: <CatalogPage /> },
			{ path: 'cart', element: <CartPage /> },
			{ path: 'checkout', element: <CheckoutPage /> },
		],
	},
])

ReactDOM.createRoot(document.getElementById('root')!).render(
	<React.StrictMode>
		<UnitProvider>
			<RouterProvider router={router} />
		</UnitProvider>
	</React.StrictMode>,
)
