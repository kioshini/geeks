import { Outlet, Link, useLocation } from 'react-router-dom';
import { Telegram } from '../lib/telegram';
import { useEffect, useState } from 'react';
import { useCartStore } from '../store/cart';
import { UnitToggle } from '../components/UnitToggle';
import { useUnit } from '../contexts/UnitContext';
import { CartIcon } from '../components/catalog/CartIcon';
import { Menu, X } from 'lucide-react';

export function AppLayout() {
	const location = useLocation();
    const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
    const setUserId = useCartStore(s => s.setUserId);
    const loadCart = useCartStore(s => s.loadCart);
    const cart = useCartStore(s => s.cart);
    const { selectedUnit, setSelectedUnit } = useUnit();
    
    // Вычисляем общее количество товаров в корзине
    const totalItems = cart?.items.reduce((total, item) => total + item.quantity, 0) || 0;

	useEffect(() => {
		Telegram.ready();
		Telegram.expand();
        // Determine user id from Telegram or fallback demo id for local testing
        const tgUser = Telegram.user();
        const demoStored = typeof window !== 'undefined' ? window.localStorage.getItem('demoUserId') : null;
        let uid: number | null = tgUser?.id ?? (demoStored ? Number(demoStored) : null);
        if (!uid) {
            uid = 999001; // demo user id
            try { window.localStorage.setItem('demoUserId', String(uid)); } catch {}
        }
        setUserId(uid);
        loadCart();
	}, []);

	return (
		<div className="min-h-screen bg-grayMedium flex flex-col">
			<header className="bg-dark shadow-lg border-b border-grayDark sticky top-0 z-50">
				<div className="max-w-7xl mx-auto px-2 sm:px-4 lg:px-8">
					<div className="flex items-center justify-between h-14 sm:h-16">
						{/* Logo */}
						<div className="flex items-center gap-2 sm:gap-4">
							<div className="h-8 w-8 sm:h-10 sm:w-10 rounded-lg bg-primary flex items-center justify-center text-white font-bold text-sm sm:text-lg">
								T
							</div>
							<div>
								<h1 className="text-lg sm:text-xl font-bold text-white">TMK Mini App</h1>
								<p className="text-xs sm:text-sm text-grayLight hidden sm:block">Steel Solutions</p>
							</div>
						</div>

						{/* Desktop Navigation */}
						<nav className="hidden md:flex items-center gap-2">
							<Link to="/" className={navLinkCls(location.pathname === '/')}>
								<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
									<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
								</svg>
								<span>Каталог</span>
							</Link>
							<Link to="/cart" className={`flex items-center gap-2 px-4 py-2 rounded-lg text-sm font-semibold transition-colors ${
								location.pathname === '/cart'
									? 'bg-primary text-white'
									: 'text-grayLight hover:bg-grayDark hover:text-white'
							}`}>
								<CartIcon 
									totalItems={totalItems} 
								/>
								<span>Корзина</span>
							</Link>
							<Link to="/checkout" className={navLinkCls(location.pathname === '/checkout')}>
								<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
									<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
								</svg>
								<span>Оформление</span>
							</Link>
						</nav>

						{/* Mobile Menu Button */}
						<button
							onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
							className="md:hidden p-2 rounded-md text-grayLight hover:bg-grayDark transition-colors"
							aria-label="Открыть меню"
						>
							{isMobileMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
						</button>
					</div>

					{/* Mobile Navigation */}
					{isMobileMenuOpen && (
						<div className="md:hidden border-t border-grayDark py-4">
							<div className="flex flex-col space-y-2">
								<Link 
									to="/" 
									className={`flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-semibold transition-colors ${
										location.pathname === '/'
											? 'bg-primary text-white'
											: 'text-grayLight hover:bg-grayDark'
									}`}
									onClick={() => setIsMobileMenuOpen(false)}
								>
									<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
										<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
									</svg>
									<span>Каталог</span>
								</Link>
								<Link 
									to="/cart" 
									className={`flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-semibold transition-colors ${
										location.pathname === '/cart'
											? 'bg-primary text-white'
											: 'text-grayLight hover:bg-grayDark'
									}`}
									onClick={() => setIsMobileMenuOpen(false)}
								>
									<CartIcon 
										totalItems={totalItems} 
									/>
									<span>Корзина</span>
								</Link>
								<Link 
									to="/checkout" 
									className={`flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-semibold transition-colors ${
										location.pathname === '/checkout'
											? 'bg-primary text-white'
											: 'text-grayLight hover:bg-grayDark'
									}`}
									onClick={() => setIsMobileMenuOpen(false)}
								>
									<svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
										<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
									</svg>
									<span>Оформление</span>
								</Link>
							</div>
							
							{/* Mobile Unit Toggle */}
							<div className="mt-4 pt-4 border-t border-grayDark">
								<UnitToggle
									selectedUnit={selectedUnit}
									onUnitChange={setSelectedUnit}
									className="w-full"
								/>
							</div>
						</div>
					)}
				</div>
			</header>
			<main className="flex-1">
				<Outlet />
			</main>
		</div>
	);
}

function navLinkCls(active: boolean) {
	return `flex items-center gap-2 px-4 py-2 rounded-lg text-sm font-semibold transition-colors ${
		active
			? 'bg-primary text-white'
			: 'text-grayLight hover:bg-grayDark hover:text-white'
	}`
}
