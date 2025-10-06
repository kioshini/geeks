import { type FormEvent, useEffect, useState } from 'react';
import { Api } from '../lib/api';
import type { CreateOrderDto } from '../lib/api';
import { useCartStore } from '../store/cart';
import { Telegram } from '../lib/telegram';

export function CheckoutPage() {
	const { cart, userId, setUserId, loadCart } = useCartStore();
	const [firstName, setFirstName] = useState('');
	const [lastName, setLastName] = useState('');
	const [phone, setPhone] = useState('');
	const [email, setEmail] = useState('');
	const [notes, setNotes] = useState('');
	const [loading, setLoading] = useState(false);
	const [message, setMessage] = useState<string | null>(null);
	const [human, setHuman] = useState(false);

	useEffect(() => {
		const u = Telegram.user();
		if (u?.id) setUserId(u.id);
		if (u?.first_name) setFirstName(u.first_name);
		if (u?.last_name) setLastName(u.last_name);
		loadCart();
	}, [setUserId, loadCart]);

	async function onSubmit(e: FormEvent) {
		e.preventDefault();
		if (!cart || !userId) return;
		setLoading(true);
		setMessage(null);
		try {
			const body: CreateOrderDto = {
				userId,
				firstName: firstName || 'Пользователь',
				lastName: lastName || '',
				phone: phone || 'N/A',
				email: email || undefined,
				items: cart.items.map(i => ({ productId: i.productId, quantity: i.quantity })),
				notes: notes || undefined,
			};
			if (!human) {
				setMessage('Подтвердите, что вы не робот');
				return;
			}
			const nonce = Math.random().toString(36).slice(2) + Date.now().toString(36);
			const order = await Api.createOrder(body, nonce);
			setMessage(`Заказ #${order.id} создан на сумму ${order.totalPrice.toFixed(2)} ₽`);
		} catch (e: any) {
			setMessage(e?.response?.data || e?.message || 'Ошибка при оформлении заказа');
		} finally {
			setLoading(false);
		}
	}

	if (!cart) return <div className="py-10 text-center text-sm text-steel-500">Корзина пуста</div>;

	return (
		<div className="max-w-4xl mx-auto space-y-8">
			{/* Header */}
			<div className="text-center">
				<h1 className="text-4xl font-bold text-gray-900 mb-2">Оформление заказа</h1>
				<p className="text-lg text-gray-600 mb-6">Заполните данные для завершения покупки</p>
				<div className="inline-flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-green-100 to-green-200 text-green-800 rounded-full text-sm font-semibold border border-green-300">
					<svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
						<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
					</svg>
					Безопасное оформление заказа
				</div>
			</div>

			{/* Order Summary */}
			<div className="bg-white/80 backdrop-blur-sm rounded-2xl border border-gray-200/50 p-6 shadow-lg">
				<h2 className="text-2xl font-bold text-gray-900 mb-6 flex items-center gap-2">
					<svg className="w-4 h-4 text-orange-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
						<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
					</svg>
					Ваш заказ
				</h2>
				<div className="space-y-4">
					{cart.items.map(item => (
						<div key={item.id} className="flex items-center justify-between py-4 px-4 bg-gray-50 rounded-xl border border-gray-200">
							<div className="flex-1">
								<div className="text-lg font-bold text-gray-900 mb-1">
									{item.product?.name || `Товар #${item.productId}`}
								</div>
								<div className="text-sm text-gray-600">
									Материал: {item.product?.material || 'Не указан'}
								</div>
								<div className="text-sm text-gray-500 mt-1">
									{item.quantity} шт × {item.price.toFixed(2)} ₽
								</div>
							</div>
							<div className="text-xl font-bold text-gray-900">
								{item.totalPrice.toFixed(2)} ₽
							</div>
						</div>
					))}
				</div>
				<div className="flex items-center justify-between pt-6 mt-6 border-t-2 border-gray-200">
					<div className="text-2xl font-bold text-gray-900">Итого</div>
					<div className="text-3xl font-bold text-orange-600">
						{cart.totalPrice.toFixed(2)} ₽
					</div>
				</div>
			</div>

			{/* Form */}
			<form onSubmit={onSubmit} className="space-y-6">
				{/* Message */}
				{message && (
					<div className={`p-4 rounded-xl text-sm font-semibold ${
						message.includes('создан') 
							? 'bg-gradient-to-r from-green-50 to-green-100 border-2 border-green-200 text-green-800'
							: 'bg-gradient-to-r from-red-50 to-red-100 border-2 border-red-200 text-red-800'
					}`}>
						<div className="flex items-center gap-2">
							<svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
								<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d={message.includes('создан') ? "M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" : "M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"} />
							</svg>
							{message}
						</div>
					</div>
				)}

				{/* Personal Information */}
				<div className="bg-white/80 backdrop-blur-sm rounded-2xl border border-gray-200/50 p-6 shadow-lg">
					<h2 className="text-2xl font-bold text-gray-900 mb-6 flex items-center gap-2">
						<svg className="w-4 h-4 text-orange-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
							<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
						</svg>
						Контактные данные
					</h2>
					<div className="grid grid-cols-1 md:grid-cols-2 gap-6">
						<div>
							<label className="block text-sm font-bold text-gray-700 mb-2">Имя *</label>
							<input 
								value={firstName} 
								onChange={e => setFirstName(e.target.value)} 
								placeholder="Введите имя" 
								required
								className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent transition-all duration-200"
							/>
						</div>
						<div>
							<label className="block text-sm font-bold text-gray-700 mb-2">Фамилия</label>
							<input 
								value={lastName} 
								onChange={e => setLastName(e.target.value)} 
								placeholder="Введите фамилию" 
								className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent transition-all duration-200"
							/>
						</div>
					</div>
					<div className="mt-6">
						<label className="block text-sm font-bold text-gray-700 mb-2">Телефон *</label>
						<input 
							value={phone} 
							onChange={e => setPhone(e.target.value)} 
							placeholder="+7 (999) 123-45-67" 
							required
							className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent transition-all duration-200"
						/>
					</div>
					<div className="mt-6">
						<label className="block text-sm font-bold text-gray-700 mb-2">Email</label>
						<input 
							value={email} 
							onChange={e => setEmail(e.target.value)} 
							placeholder="example@email.com" 
							type="email"
							className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent transition-all duration-200"
						/>
					</div>
				</div>

				{/* Additional Information */}
				<div className="bg-white/80 backdrop-blur-sm rounded-2xl border border-gray-200/50 p-6 shadow-lg">
					<h2 className="text-2xl font-bold text-gray-900 mb-6 flex items-center gap-2">
						<svg className="w-4 h-4 text-orange-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
							<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
						</svg>
						Дополнительно
					</h2>
					<div>
						<label className="block text-sm font-bold text-gray-700 mb-2">Комментарий к заказу</label>
						<textarea 
							value={notes} 
							onChange={e => setNotes(e.target.value)} 
							placeholder="Особые требования, адрес доставки и т.д." 
							rows={4}
							className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent resize-none transition-all duration-200"
						/>
					</div>
				</div>

				{/* Human Verification */}
				<div className="bg-white/80 backdrop-blur-sm rounded-2xl border border-gray-200/50 p-6 shadow-lg">
					<label className="flex items-center gap-3 text-sm font-bold text-gray-700 cursor-pointer">
						<input 
							type="checkbox" 
							checked={human} 
							onChange={e => setHuman(e.target.checked)} 
							className="w-5 h-5 text-orange-600 border-gray-300 rounded focus:ring-orange-500 focus:ring-2"
						/>
						<svg className="w-4 h-4 text-orange-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
							<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
						</svg>
						Я подтверждаю, что я не робот
					</label>
				</div>

				{/* Submit Button */}
				<button 
					disabled={loading || !human} 
					className="w-full px-8 py-4 bg-gradient-to-r from-orange-500 to-orange-600 text-white font-bold text-lg rounded-xl hover:from-orange-600 hover:to-orange-700 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 shadow-lg hover:shadow-xl transform hover:scale-105 disabled:transform-none disabled:hover:scale-100"
				>
					{loading ? (
						<div className="flex items-center justify-center gap-2">
							<svg className="animate-spin w-4 h-4" fill="none" viewBox="0 0 24 24">
								<circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
								<path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
							</svg>
							Обработка заказа...
						</div>
					) : (
						<div className="flex items-center justify-center gap-2">
							<svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
								<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
							</svg>
							Оформить заказ
						</div>
					)}
				</button>
			</form>
		</div>
	);
}
