import { useEffect } from 'react';
import { useCartStore } from '../store/cart';
import { Telegram } from '../lib/telegram';

export function CartPage() {
	const { cart, loadCart, update, remove, clear, userId, setUserId, loading } = useCartStore();

	useEffect(() => {
		if (!userId) {
			const u = Telegram.user();
			if (u?.id) setUserId(u.id);
		}
	}, [userId, setUserId]);

	useEffect(() => {
		if (userId) loadCart();
	}, [userId, loadCart]);

	if (!userId) return <div className="py-10 text-center text-sm text-steel-500">Ожидание Telegram пользователя...</div>;
	if (!cart) return <div className="py-10 text-center text-sm text-steel-500">{loading ? 'Загрузка...' : 'Корзина пуста'}</div>;

	return (
		<div className="space-y-8">
			{/* Header */}
			<div className="text-center">
				<h1 className="text-4xl font-bold text-gray-900 mb-2">Корзина покупок</h1>
				<p className="text-lg text-gray-600 mb-6">Проверьте выбранные товары перед оформлением заказа</p>
				<div className="inline-flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-orange-100 to-orange-200 text-orange-800 rounded-full text-sm font-semibold border border-orange-300">
					<svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
						<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 13m0 0l-2.5 5M7 13l2.5 5m6-5v6a2 2 0 11-4 0v-6m4 0V9a2 2 0 00-2-2H9a2 2 0 00-2 2v4.01" />
					</svg>
					{cart.totalItems} товаров на сумму {cart.totalPrice.toFixed(2)} ₽
				</div>
			</div>

			{/* Cart Items */}
			<div className="space-y-6">
				{cart.items.map(item => (
					<div key={item.id} className="group bg-white rounded-2xl border border-gray-200/50 p-6 hover:shadow-xl hover:shadow-gray-200/50 transition-all duration-300">
						<div className="flex items-start justify-between">
							<div className="flex-1 min-w-0">
								<h3 className="text-xl font-bold text-gray-900 mb-3 group-hover:text-orange-600 transition-colors">
									{item.product?.name || `Товар #${item.productId}`}
								</h3>
								<div className="flex items-center gap-4 mb-4">
									<div className="text-lg font-semibold text-gray-700">
										Цена: {item.price.toFixed(2)} ₽ за шт
									</div>
									<div className="text-sm text-gray-500">
										Материал: {item.product?.material || 'Не указан'}
									</div>
								</div>
								
								{/* Quantity Controls */}
								<div className="flex items-center gap-3">
									<button 
										onClick={() => update(item.id, { quantity: Math.max(1, item.quantity - 1) })} 
										className="w-10 h-10 rounded-xl border-2 border-gray-300 flex items-center justify-center hover:bg-gray-50 hover:border-orange-500 transition-all duration-200 font-bold text-lg"
									>
										<svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
											<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 12H4" />
										</svg>
									</button>
									<span className="w-16 text-center font-bold text-lg bg-gray-100 rounded-xl py-2">{item.quantity}</span>
									<button 
										onClick={() => update(item.id, { quantity: item.quantity + 1 })} 
										className="w-10 h-10 rounded-xl border-2 border-gray-300 flex items-center justify-center hover:bg-gray-50 hover:border-orange-500 transition-all duration-200 font-bold text-lg"
									>
										<svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
											<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
										</svg>
									</button>
								</div>
							</div>
							
							<div className="flex flex-col items-end gap-3">
								<div className="text-2xl font-bold text-gray-900">
									{item.totalPrice.toFixed(2)} ₽
								</div>
								<button 
									onClick={() => remove(item.id)} 
									className="px-4 py-2 bg-gradient-to-r from-red-500 to-red-600 text-white text-sm font-bold rounded-xl hover:from-red-600 hover:to-red-700 transition-all duration-200 shadow-lg hover:shadow-xl transform hover:scale-105"
								>
									<svg className="w-3 h-3 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
										<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
									</svg>
									Удалить
								</button>
							</div>
						</div>
					</div>
				))}
			</div>

			{/* Summary */}
			<div className="bg-white/80 backdrop-blur-sm rounded-2xl border border-gray-200/50 p-8 shadow-lg">
				<div className="text-center mb-6">
					<h2 className="text-2xl font-bold text-gray-900 mb-2">Итоговая сумма</h2>
					<div className="text-4xl font-bold text-orange-600 mb-2">
						{cart.totalPrice.toFixed(2)} ₽
					</div>
					<div className="text-lg text-gray-600">
						{cart.totalItems} товаров
					</div>
				</div>
				
				{/* Action Buttons */}
				<div className="flex flex-col sm:flex-row gap-4">
					<button 
						onClick={() => clear()} 
						className="flex-1 px-6 py-3 border-2 border-gray-300 text-gray-700 font-bold rounded-xl hover:bg-gray-50 hover:border-gray-400 transition-all duration-200 shadow-lg hover:shadow-xl transform hover:scale-105"
					>
						<svg className="w-4 h-4 inline mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
							<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
						</svg>
						Очистить корзину
					</button>
					<a 
						href="/checkout" 
						className="flex-1 px-6 py-3 bg-gradient-to-r from-orange-500 to-orange-600 text-white font-bold rounded-xl hover:from-orange-600 hover:to-orange-700 transition-all duration-200 text-center shadow-lg hover:shadow-xl transform hover:scale-105"
					>
						<svg className="w-4 h-4 inline mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
							<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
						</svg>
						Оформить заказ
					</a>
				</div>
			</div>
		</div>
	);
}
