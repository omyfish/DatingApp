import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolver/member-detail.resolver';
import { MemberListResolver } from './_resolver/member-list.resolver';

export const appRoute: Routes = [
    { path: '', component: HomeComponent},
    { path: '', children: [
        { path: 'members', component: MemberListComponent, resolve: { users: MemberListResolver} },
        { path: 'members/:id', component: MemberDetailComponent, resolve: { user: MemberDetailResolver} },
        { path: 'messages', component: MessagesComponent },
        { path: 'lists', component: ListsComponent },
    ],
    runGuardsAndResolvers: 'always',
    canActivate : [AuthGuard]},
    { path: '**', redirectTo: '', pathMatch: 'full'},
];

